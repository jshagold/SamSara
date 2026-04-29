using System;
using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using Samsara.Core.AssetLoading;
using Samsara.Core.Navigation;
using Samsara.Core.Popup;
using Samsara.Features.Character.MasterData;
using Samsara.Core.Tree;
using Samsara.Features.EvolutionTreeScene.Domain;
using Samsara.Features.EvolutionTreeScene.Presentation.Popup;
using UnityEngine;

namespace Samsara.Features.EvolutionTreeScene.Presentation
{
    public class EvolutionTreePresenter : IDisposable
    {
        private readonly string _logClass = $"[{nameof(EvolutionTreePresenter)}]";

        private readonly EvolutionTreeUseCase _useCase;
        private readonly TreeLayoutCalculator _layoutCalculator;
        private readonly EvolutionTreeView    _view;
        private readonly ISceneNavigator      _sceneNavigator;
        private readonly IPopupManager        _popupManager;
        private readonly ISpriteLoader        _spriteLoader;
        // Index order: [0]=current, [1]=evolvable, [2]=reachable, [3]=locked, [4]=hidden, [5]=questionMark
        private readonly string[]             _uiSpriteKeys;

        private EvolutionNodeSO[] _allNodes;
        private EvolutionNodeSO   _currentNode;
        private EvolutionNodeSO   _selectedNode;

        // nodeId → 로드된 노드 아이콘 스프라이트 캐시
        private readonly Dictionary<string, Sprite> _nodeIconCache = new();

        // 6 frame Sprite 인스턴스 필드 (Patch-001 §9-5 — Presenter mapping 책임)
        private Sprite _currentFrameSprite;
        private Sprite _evolvableFrameSprite;
        private Sprite _reachableFrameSprite;
        private Sprite _lockedFrameSprite;
        private Sprite _hiddenFrameSprite;
        private Sprite _questionMarkSprite;

        public EvolutionTreePresenter(
            EvolutionTreeUseCase useCase,
            TreeLayoutCalculator layoutCalculator,
            EvolutionTreeView    view,
            ISceneNavigator      sceneNavigator,
            IPopupManager        popupManager,
            ISpriteLoader        spriteLoader,
            string[]             uiSpriteKeys)
        {
            _useCase          = useCase;
            _layoutCalculator = layoutCalculator;
            _view             = view;
            _sceneNavigator   = sceneNavigator;
            _popupManager     = popupManager;
            _spriteLoader     = spriteLoader;
            _uiSpriteKeys     = uiSpriteKeys;
        }

        public void Initialize()
        {
            InitializeAsync().Forget();
        }

        private async UniTaskVoid InitializeAsync()
        {
            _allNodes    = _useCase.GetAllNodes();
            _currentNode = _useCase.FindCurrentNode();

            // Load 6 UI frame sprites before building the tree (Patch-001 §9-5)
            _currentFrameSprite   = _uiSpriteKeys != null && _uiSpriteKeys.Length > 0 ? await _spriteLoader.LoadSpriteAsync(_uiSpriteKeys[0]) : null;
            _evolvableFrameSprite = _uiSpriteKeys != null && _uiSpriteKeys.Length > 1 ? await _spriteLoader.LoadSpriteAsync(_uiSpriteKeys[1]) : null;
            _reachableFrameSprite = _uiSpriteKeys != null && _uiSpriteKeys.Length > 2 ? await _spriteLoader.LoadSpriteAsync(_uiSpriteKeys[2]) : null;
            _lockedFrameSprite    = _uiSpriteKeys != null && _uiSpriteKeys.Length > 3 ? await _spriteLoader.LoadSpriteAsync(_uiSpriteKeys[3]) : null;
            _hiddenFrameSprite    = _uiSpriteKeys != null && _uiSpriteKeys.Length > 4 ? await _spriteLoader.LoadSpriteAsync(_uiSpriteKeys[4]) : null;
            _questionMarkSprite   = _uiSpriteKeys != null && _uiSpriteKeys.Length > 5 ? await _spriteLoader.LoadSpriteAsync(_uiSpriteKeys[5]) : null;

            var layoutResult = _layoutCalculator.CalculateLayout(_allNodes);
            _view.TreeScrollView.BuildTree(layoutResult);

            // Setup each node view — load icon sprite + apply mapping (Patch-001 §9-4 C1 self-mapping)
            foreach (var node in _allNodes)
            {
                var nodeView = _view.TreeScrollView.GetNodeView(node.NodeId);
                if (nodeView == null) continue;

                var iconSprite = await _spriteLoader.LoadSpriteAsync(node.NodeIconSpriteKey);
                _nodeIconCache[node.NodeId] = iconSprite;
                nodeView.Setup(node.NodeId, iconSprite);

                var state = _useCase.ClassifyNodeState(node, _currentNode);
                var frameSprite = MapStateToFrameSprite(state);
                nodeView.SetFrameSprite(frameSprite);

                if (state == EvolutionNodeState.Hidden)
                {
                    nodeView.SetIconSprite(_questionMarkSprite);
                }
            }

            _view.TreeScrollView.ScrollToNode(_currentNode.NodeId);

            // Subscribe events
            var allNodeViews = _view.TreeScrollView.GetAllNodeViews();
            foreach (var nodeView in allNodeViews)
            {
                nodeView.OnNodeClicked += HandleNodeClicked;
            }

            _view.NodeDescriptionPopupView.OnEvolveClicked += HandleEvolveClicked;
            _view.NodeDescriptionPopupView.OnCloseClicked  += HandleCloseClicked;
            _view.ReincarnationButtonView.OnReincarnationClicked += HandleReincarnationClicked;
            _view.BackButtonView.OnBackClicked             += HandleBackClicked;
        }

        private Sprite MapStateToFrameSprite(EvolutionNodeState state)
        {
            return state switch
            {
                EvolutionNodeState.Current   => _currentFrameSprite,
                EvolutionNodeState.Evolvable => _evolvableFrameSprite,
                EvolutionNodeState.Reachable => _reachableFrameSprite,
                EvolutionNodeState.Locked    => _lockedFrameSprite,
                EvolutionNodeState.Hidden    => _hiddenFrameSprite,
                _ => null
            };
        }

        private void HandleNodeClicked(string nodeId) => HandleNodeClickedAsync(nodeId).Forget();

        private async UniTaskVoid HandleNodeClickedAsync(string nodeId)
        {
            EvolutionNodeSO tappedNode = null;
            foreach (var node in _allNodes)
            {
                if (node.NodeId == nodeId)
                {
                    tappedNode = node;
                    break;
                }
            }

            if (tappedNode == null) return;
            _selectedNode = tappedNode;

            var state = _useCase.ClassifyNodeState(tappedNode, _currentNode);
            bool isHiddenLocked = state == EvolutionNodeState.Hidden;

            // Build stats text
            var statsBuilder = new StringBuilder();
            statsBuilder.AppendLine($"HP: {tappedNode.BaseStats.Hp}");
            statsBuilder.AppendLine($"Strength: {tappedNode.BaseStats.Strength}");
            statsBuilder.AppendLine($"Toughness: {tappedNode.BaseStats.Toughness}");
            statsBuilder.Append($"Agility: {tappedNode.BaseStats.Agility}");

            // Build conditions text
            var conditionsBuilder = new StringBuilder();
            if (tappedNode.UnlockConditions != null)
            {
                foreach (var condition in tappedNode.UnlockConditions)
                {
                    conditionsBuilder.AppendLine($"{condition.StatType} >= {condition.RequiredValue}");
                }
            }

            // 노드 아이콘: 캐시에서 조회
            _nodeIconCache.TryGetValue(nodeId, out var nodeIcon);

            // 스킬 아이콘: 스킬 SO 로드 후 각 아이콘 스프라이트 로드
            var skills     = _useCase.GetSkillsForNode(tappedNode.SkillIds);
            var skillIcons = new Sprite[skills.Length];
            for (int i = 0; i < skills.Length; i++)
                skillIcons[i] = await _spriteLoader.LoadSpriteAsync(skills[i].IconSpriteKey);

            var data = new NodeDescriptionData
            {
                NodeIcon       = nodeIcon,
                CharacterName  = tappedNode.CharacterName,
                StatsText      = statsBuilder.ToString(),
                ConditionsText = conditionsBuilder.ToString(),
                SkillIcons     = skillIcons,
                CanEvolve      = state == EvolutionNodeState.Evolvable,
                IsHiddenLocked = isHiddenLocked
            };

            _view.NodeDescriptionPopupView.Show(data);
        }

        private void HandleEvolveClicked()
        {
            HandleEvolveAsync().Forget();
        }

        private async UniTaskVoid HandleEvolveAsync()
        {
            if (_selectedNode == null) return;

            var request = new PopupRequest("Evolution", "Are you sure you want to evolve?", "Confirm", "Cancel");
            var confirmed = await _popupManager.ShowYesNoAsync(request);
            if (!confirmed) return;

            await _useCase.ExecuteEvolutionAsync(_selectedNode);
            await _sceneNavigator.NavigateToAsync(SceneKey.Main);
        }

        private void HandleReincarnationClicked()
        {
            HandleReincarnationAsync().Forget();
        }

        private async UniTaskVoid HandleReincarnationAsync()
        {
            var request = new PopupRequest("Reincarnation", "Are you sure you want to reincarnate?", "Confirm", "Cancel");
            var confirmed = await _popupManager.ShowYesNoAsync(request);
            if (!confirmed) return;

            await _useCase.ExecuteReincarnationAsync();
            await _sceneNavigator.NavigateToAsync(SceneKey.Splash);
        }

        private void HandleCloseClicked()
        {
            _view.NodeDescriptionPopupView.Hide();
            _selectedNode = null;
        }

        private void HandleBackClicked()
        {
            _sceneNavigator.NavigateToAsync(SceneKey.CharacterInfo).Forget();
        }

        public void Dispose()
        {
            var allNodeViews = _view?.TreeScrollView?.GetAllNodeViews();
            if (allNodeViews != null)
            {
                foreach (var nodeView in allNodeViews)
                {
                    nodeView.OnNodeClicked -= HandleNodeClicked;
                }
            }

            if (_view?.NodeDescriptionPopupView != null)
            {
                _view.NodeDescriptionPopupView.OnEvolveClicked -= HandleEvolveClicked;
                _view.NodeDescriptionPopupView.OnCloseClicked  -= HandleCloseClicked;
            }

            if (_view?.ReincarnationButtonView != null)
                _view.ReincarnationButtonView.OnReincarnationClicked -= HandleReincarnationClicked;

            if (_view?.BackButtonView != null)
                _view.BackButtonView.OnBackClicked -= HandleBackClicked;
        }
    }
}
