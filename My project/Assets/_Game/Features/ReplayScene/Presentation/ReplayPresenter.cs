using System;
using System.Text;
using Cysharp.Threading.Tasks;
using Samsara.Core.AssetLoading;
using Samsara.Core.Popup;
using Samsara.Core.Tree;
using Samsara.Features.Character.MasterData;
using Samsara.Features.ReplayScene.Domain;
using Samsara.Features.ReplayScene.Presentation.Popup;
using Samsara.Features.Skill.Domain;
using UnityEngine;

namespace Samsara.Features.ReplayScene.Presentation
{
    [Serializable]
    public class StateVisualConfig
    {
        public string SelectableFrameSpriteKey;
        public string LockedFrameSpriteKey;
        public string HiddenFrameSpriteKey;
        public string QuestionMarkSpriteKey;
    }

    public class ReplayPresenter : IDisposable
    {
        private readonly string _logClass = $"[{nameof(ReplayPresenter)}]";

        private readonly IReplayUseCase             _useCase;
        private readonly ReplayView                 _view;
        private readonly IPopupManager              _popupManager;
        private readonly ISpriteLoader              _spriteLoader;
        private readonly ISkillMasterDataRepository _skillMasterDataRepo;
        private readonly TreeLayoutCalculator       _layoutCalculator;
        private readonly StateVisualConfig          _stateVisualConfig;

        // 4 frame Sprite cache (load result)
        private Sprite _selectableFrameSprite;
        private Sprite _lockedFrameSprite;
        private Sprite _hiddenFrameSprite;
        private Sprite _questionMarkSprite;

        // Internal state
        private EvolutionNodeSO[] _allNodes;
        private EvolutionNodeSO _currentNodeForPopup;
        private Sprite[] _currentSkillIcons;

        public ReplayPresenter(
            IReplayUseCase             useCase,
            ReplayView                 view,
            IPopupManager              popupManager,
            ISpriteLoader              spriteLoader,
            ISkillMasterDataRepository skillMasterDataRepo,
            TreeLayoutCalculator       layoutCalculator,
            StateVisualConfig          stateVisualConfig)
        {
            _useCase             = useCase;
            _view                = view;
            _popupManager        = popupManager;
            _spriteLoader        = spriteLoader;
            _skillMasterDataRepo = skillMasterDataRepo;
            _layoutCalculator    = layoutCalculator;
            _stateVisualConfig   = stateVisualConfig;
        }

        public void Initialize()
        {
            _view.OnOptionButtonTapped += HandleOptionButtonTapped;
            _view.NodeDescriptionPopupView.OnRestartClicked += HandleRestartClicked;
            _view.NodeDescriptionPopupView.OnCloseClicked += HandleNodeDescriptionPopupCloseClicked;
            _view.NodeDescriptionPopupView.OnSkillSlotClicked += HandleSkillSlotClicked;
            _view.SkillDescriptionPopupView.OnCloseClicked += HandleSkillDescriptionPopupCloseClicked;

            // UI text hardcoded: tracked in Project Backlog GBL-001. Subject to Phase 7 global cleanup.
            _view.GuidanceView.SetGuidanceText("재시작할 진화체를 선택하세요.");

            BuildTreeAsync().Forget();
        }

        private async UniTask BuildTreeAsync()
        {
            // 1. Load 4 SpriteKeys
            _selectableFrameSprite = await _spriteLoader.LoadSpriteAsync(_stateVisualConfig.SelectableFrameSpriteKey);
            _lockedFrameSprite     = await _spriteLoader.LoadSpriteAsync(_stateVisualConfig.LockedFrameSpriteKey);
            _hiddenFrameSprite     = await _spriteLoader.LoadSpriteAsync(_stateVisualConfig.HiddenFrameSpriteKey);
            _questionMarkSprite    = await _spriteLoader.LoadSpriteAsync(_stateVisualConfig.QuestionMarkSpriteKey);

            // 2. Get all nodes — IReadOnlyList → array conversion (existing layout calculator expects EvolutionNodeSO[])
            var allNodesList = _useCase.GetAllNodes();
            _allNodes = new EvolutionNodeSO[allNodesList.Count];
            for (int i = 0; i < allNodesList.Count; i++)
            {
                _allNodes[i] = allNodesList[i];
            }

            // 3. Build layout + tree
            var layoutResult = _layoutCalculator.CalculateLayout(_allNodes);
            _view.TreeScrollView.BuildTree(layoutResult);

            // 4. Apply mapping to each NodeView
            foreach (var node in _allNodes)
            {
                var nodeView = _view.TreeScrollView.GetNodeView(node.NodeId);
                if (nodeView == null) continue;

                var iconSprite = await _spriteLoader.LoadSpriteAsync(node.NodeIconSpriteKey);
                nodeView.Setup(node.NodeId, iconSprite);

                var state = _useCase.ClassifyNode(node);
                Sprite frameSprite = MapStateToFrameSprite(state);
                nodeView.SetFrameSprite(frameSprite);

                if (state == ReplayNodeState.Hidden)
                {
                    nodeView.SetIconSprite(_questionMarkSprite);
                }
            }

            // 5. Subscribe per-NodeView click events (TreeScrollView has no relay event — direct subscription)
            var allNodeViews = _view.TreeScrollView.GetAllNodeViews();
            foreach (var nodeView in allNodeViews)
            {
                nodeView.OnNodeClicked += HandleNodeClicked;
            }
        }

        private Sprite MapStateToFrameSprite(ReplayNodeState state)
        {
            return state switch
            {
                ReplayNodeState.Selectable => _selectableFrameSprite,
                ReplayNodeState.Locked     => _lockedFrameSprite,
                ReplayNodeState.Hidden     => _hiddenFrameSprite,
                _ => null
            };
        }

        private void HandleNodeClicked(string nodeId) => HandleNodeClickedAsync(nodeId).Forget();

        private async UniTaskVoid HandleNodeClickedAsync(string nodeId)
        {
            EvolutionNodeSO clicked = null;
            if (_allNodes != null)
            {
                foreach (var n in _allNodes)
                {
                    if (n.NodeId == nodeId) { clicked = n; break; }
                }
            }

            if (clicked == null)
            {
                Debug.LogWarning($"{_logClass} HandleNodeClickedAsync: node not found for id=[{nodeId}]");
                return;
            }

            var state = _useCase.ClassifyNode(clicked);

            var data = new ReplayNodeDescriptionData
            {
                ConditionsText = ComposeConditionsText(clicked.UnlockConditions),
            };

            if (state == ReplayNodeState.Hidden)
            {
                data.IsHiddenLocked = true;
                data.CanRestart = false;
                // CharacterName / StatsText / SkillIcons / NodeIcon not set
            }
            else if (state == ReplayNodeState.Locked)
            {
                data.IsHiddenLocked = false;
                data.CanRestart = false;
                data.CharacterName = clicked.CharacterName;
                data.NodeIcon = await _spriteLoader.LoadSpriteAsync(clicked.NodeIconSpriteKey);
            }
            else // Selectable
            {
                data.IsHiddenLocked = false;
                data.CanRestart = true;
                data.CharacterName = clicked.CharacterName;
                data.NodeIcon = await _spriteLoader.LoadSpriteAsync(clicked.NodeIconSpriteKey);
                data.StatsText = ComposeStatsText(clicked.BaseStats);
                data.SkillIcons = await LoadSkillIconsAsync(clicked.SkillIds);
            }

            _currentNodeForPopup = clicked;
            _currentSkillIcons = data.SkillIcons;
            _view.NodeDescriptionPopupView.Show(data);
        }

        private async UniTask<Sprite[]> LoadSkillIconsAsync(int[] skillIds)
        {
            if (skillIds == null || skillIds.Length == 0)
                return Array.Empty<Sprite>();

            var skills = _skillMasterDataRepo.GetSkillsByIds(skillIds);
            var sprites = new Sprite[skills.Length];
            for (int i = 0; i < skills.Length; i++)
            {
                sprites[i] = await _spriteLoader.LoadSpriteAsync(skills[i].IconSpriteKey);
            }
            return sprites;
        }

        private string ComposeStatsText(CharacterStatsSO stats)
        {
            if (stats == null) return string.Empty;
            // UI text hardcoded: tracked in Project Backlog GBL-001. Subject to Phase 7 global cleanup.
            return $"체력 {stats.Hp}\n힘 {stats.Strength}\n강인함 {stats.Toughness}\n민첩 {stats.Agility}";
        }

        private string ComposeConditionsText(StatCondition[] conditions)
        {
            if (conditions == null || conditions.Length == 0)
                // UI text hardcoded: tracked in Project Backlog GBL-001. Subject to Phase 7 global cleanup.
                return "해금 조건 없음";

            var sb = new StringBuilder();
            for (int i = 0; i < conditions.Length; i++)
            {
                // UI text hardcoded: tracked in Project Backlog GBL-001. Subject to Phase 7 global cleanup.
                sb.AppendLine($"- {conditions[i].StatType} >= {conditions[i].RequiredValue}");
            }
            return sb.ToString();
        }

        private void HandleSkillSlotClicked(int index) => HandleSkillSlotClickedAsync(index).Forget();

        private async UniTaskVoid HandleSkillSlotClickedAsync(int index)
        {
            if (_currentNodeForPopup == null || _currentNodeForPopup.SkillIds == null)
                return;

            if (index < 0 || index >= _currentNodeForPopup.SkillIds.Length)
            {
                Debug.LogWarning($"{_logClass} HandleSkillSlotClickedAsync: index [{index}] out of range");
                return;
            }

            var skillId = _currentNodeForPopup.SkillIds[index];
            var skills = _skillMasterDataRepo.GetSkillsByIds(new[] { skillId });
            if (skills == null || skills.Length == 0)
            {
                Debug.LogWarning($"{_logClass} HandleSkillSlotClickedAsync: skill not found for id [{skillId}]");
                return;
            }

            var skill = skills[0];
            var iconSprite = (_currentSkillIcons != null && index < _currentSkillIcons.Length)
                ? _currentSkillIcons[index]
                : await _spriteLoader.LoadSpriteAsync(skill.IconSpriteKey);

            var data = new ReplaySkillDescriptionData
            {
                Icon = iconSprite,
                SkillName = skill.SkillName,
                Description = skill.Description,
                Damage = skill.Damage,
            };

            _view.SkillDescriptionPopupView.Show(data);
        }

        private void HandleNodeDescriptionPopupCloseClicked()
        {
            _view.NodeDescriptionPopupView.Hide();
            _currentNodeForPopup = null;
            _currentSkillIcons = null;
        }

        private void HandleSkillDescriptionPopupCloseClicked()
        {
            _view.SkillDescriptionPopupView.Hide();
        }

        private void HandleRestartClicked() => HandleRestartClickedAsync().Forget();

        private async UniTaskVoid HandleRestartClickedAsync()
        {
            if (_currentNodeForPopup == null)
            {
                Debug.LogWarning($"{_logClass} HandleRestartClicked: _currentNodeForPopup is null");
                return;
            }

            var selectedNode = _currentNodeForPopup;

            // UI text hardcoded: tracked in Project Backlog GBL-001. Subject to Phase 7 global cleanup.
            var request = new PopupRequest(
                "재시작 확정",
                $"{selectedNode.CharacterName}(으)로 새 런을 시작합니다.",
                "재시작",
                "취소");

            bool confirmed = await _popupManager.ShowYesNoAsync(request);

            if (!confirmed)
            {
                return;
            }

            await _useCase.ExecuteRestartAsync(selectedNode.NodeId);
        }

        private void HandleOptionButtonTapped()
        {
            // 1차 개발 미구현 — Plan §11 OQ-G8 (Option menu 미정).
        }

        public void Dispose()
        {
            if (_view != null)
            {
                _view.OnOptionButtonTapped -= HandleOptionButtonTapped;

                if (_view.NodeDescriptionPopupView != null)
                {
                    _view.NodeDescriptionPopupView.OnRestartClicked -= HandleRestartClicked;
                    _view.NodeDescriptionPopupView.OnCloseClicked -= HandleNodeDescriptionPopupCloseClicked;
                    _view.NodeDescriptionPopupView.OnSkillSlotClicked -= HandleSkillSlotClicked;
                }

                if (_view.SkillDescriptionPopupView != null)
                {
                    _view.SkillDescriptionPopupView.OnCloseClicked -= HandleSkillDescriptionPopupCloseClicked;
                }

                var allNodeViews = _view.TreeScrollView?.GetAllNodeViews();
                if (allNodeViews != null)
                {
                    foreach (var nodeView in allNodeViews)
                    {
                        if (nodeView != null)
                            nodeView.OnNodeClicked -= HandleNodeClicked;
                    }
                }
            }
        }
    }
}
