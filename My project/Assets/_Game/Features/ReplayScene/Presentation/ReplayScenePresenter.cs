using System;
using System.Collections.Generic;
using System.Text;
using Cysharp.Threading.Tasks;
using Samsara.Core.AssetLoading;
using Samsara.Core.Navigation;
using Samsara.Core.Popup;
using Samsara.Core.Tree;
using Samsara.Features.Character.MasterData;
using Samsara.Features.ReplayScene.Domain;
using Samsara.Features.ReplayScene.Presentation.Popup;
using UnityEngine;

namespace Samsara.Features.ReplayScene.Presentation
{
    public class ReplayScenePresenter : IDisposable
    {
        private readonly string _logClass = $"[{nameof(ReplayScenePresenter)}]";

        private readonly ReplaySceneUseCase   _useCase;
        private readonly TreeLayoutCalculator _layoutCalculator;
        private readonly ReplaySceneView      _view;
        private readonly ISceneNavigator      _sceneNavigator;
        private readonly IPopupManager        _popupManager;
        private readonly ISpriteLoader        _spriteLoader;
        // Index order: [0]=evolvable (reused for Selectable, RQ-14), [1]=locked, [2]=hidden, [3]=questionMark
        private readonly string[]             _frameSpriteKeys;

        private EvolutionNodeSO[]                    _allNodes;
        private EvolutionNodeSO                      _selectedNode;
        private readonly Dictionary<string, Sprite>  _nodeIconCache = new Dictionary<string, Sprite>();

        public ReplayScenePresenter(
            ReplaySceneUseCase   useCase,
            TreeLayoutCalculator layoutCalculator,
            ReplaySceneView      view,
            ISceneNavigator      sceneNavigator,
            IPopupManager        popupManager,
            ISpriteLoader        spriteLoader,
            string[]             frameSpriteKeys)
        {
            _useCase          = useCase;
            _layoutCalculator = layoutCalculator;
            _view             = view;
            _sceneNavigator   = sceneNavigator;
            _popupManager     = popupManager;
            _spriteLoader     = spriteLoader;
            _frameSpriteKeys  = frameSpriteKeys;
        }

        public void Initialize()
        {
            InitializeAsync().Forget();
        }

        private async UniTask InitializeAsync()
        {
            _allNodes = _useCase.GetAllNodes();

            // Preload 4 frame sprites before building the tree
            var evolvableFrame  = _frameSpriteKeys != null && _frameSpriteKeys.Length > 0 ? await _spriteLoader.LoadSpriteAsync(_frameSpriteKeys[0]) : null;
            var lockedFrame     = _frameSpriteKeys != null && _frameSpriteKeys.Length > 1 ? await _spriteLoader.LoadSpriteAsync(_frameSpriteKeys[1]) : null;
            var hiddenFrame     = _frameSpriteKeys != null && _frameSpriteKeys.Length > 2 ? await _spriteLoader.LoadSpriteAsync(_frameSpriteKeys[2]) : null;
            var questionMark    = _frameSpriteKeys != null && _frameSpriteKeys.Length > 3 ? await _spriteLoader.LoadSpriteAsync(_frameSpriteKeys[3]) : null;

            var layoutResult = _layoutCalculator.CalculateLayout(_allNodes);
            _view.TreeScrollView.BuildTree(layoutResult);

            foreach (var node in _allNodes)
            {
                var nodeView = _view.TreeScrollView.GetNodeView(node.NodeId);
                if (nodeView == null) continue;

                var iconSprite = await _spriteLoader.LoadSpriteAsync(node.NodeIconSpriteKey);
                _nodeIconCache[node.NodeId] = iconSprite;
                nodeView.Setup(node.NodeId, iconSprite);

                // Pass null for current/reachable (unused in ReplayScene)
                nodeView.SetUISprites(null, evolvableFrame, null, lockedFrame, hiddenFrame, questionMark);

                var state = _useCase.ClassifyNodeStateForReplay(node);
                // Selectable reuses Evolvable frame (RQ-14) — NodeView has no Selectable case
                var displayState = state == NodeState.Selectable ? NodeState.Evolvable : state;
                nodeView.SetNodeState(displayState);
            }

            var allNodeViews = _view.TreeScrollView.GetAllNodeViews();
            foreach (var nodeView in allNodeViews)
            {
                nodeView.OnNodeClicked += HandleNodeClicked;
            }

            _view.PopupView.OnRestartClicked += HandleRestartRequested;
            _view.PopupView.OnCloseClicked   += HandlePopupCloseRequested;

            Debug.Log($"{_logClass} InitializeAsync 완료 — nodes:{_allNodes.Length}");
        }

        private void HandleNodeClicked(string nodeId)
        {
            HandleNodeClickedAsync(nodeId).Forget();
        }

        private async UniTaskVoid HandleNodeClickedAsync(string nodeId)
        {
            EvolutionNodeSO clicked = null;
            foreach (var node in _allNodes)
            {
                if (node.NodeId == nodeId) { clicked = node; break; }
            }

            if (clicked == null)
            {
                Debug.LogWarning($"{_logClass} HandleNodeClicked: node not found for id=[{nodeId}]");
                return;
            }

            var state = _useCase.ClassifyNodeStateForReplay(clicked);

            if (state == NodeState.Locked || state == NodeState.Hidden) return;

            _selectedNode = clicked;

            var statsBuilder = new StringBuilder();
            statsBuilder.AppendLine($"HP: {clicked.BaseStats.Hp}");
            statsBuilder.AppendLine($"Strength: {clicked.BaseStats.Strength}");
            statsBuilder.AppendLine($"Toughness: {clicked.BaseStats.Toughness}");
            statsBuilder.Append($"Agility: {clicked.BaseStats.Agility}");

            var conditionsBuilder = new StringBuilder();
            if (clicked.UnlockConditions != null)
            {
                foreach (var condition in clicked.UnlockConditions)
                {
                    conditionsBuilder.AppendLine($"{condition.StatType} >= {condition.RequiredValue}");
                }
            }

            _nodeIconCache.TryGetValue(nodeId, out var nodeIcon);

            var skills     = _useCase.GetSkillsForNode(clicked.SkillIds);
            var skillIcons = new Sprite[skills.Length];
            for (int i = 0; i < skills.Length; i++)
                skillIcons[i] = await _spriteLoader.LoadSpriteAsync(skills[i].IconSpriteKey);

            var data = new NodeDescriptionData
            {
                NodeIcon      = nodeIcon,
                CharacterName = clicked.CharacterName,
                StatsText     = statsBuilder.ToString(),
                ConditionsText = conditionsBuilder.ToString(),
                SkillIcons    = skillIcons,
                IsHiddenLocked = false
            };

            _view.PopupView.Show(data, isRestartEnabled: true);
        }

        private async void HandleRestartRequested()
        {
            if (_selectedNode == null) return;

            // UI text hardcoding: tracked by Project Backlog GBL-001. Phase 7 global cleanup target.
            var request = new PopupRequest(
                "재시작 확정",
                $"{_selectedNode.CharacterName}(으)로 새 런을 시작합니다.",
                "재시작",
                "취소");

            bool confirmed = await _popupManager.ShowYesNoAsync(request);

            if (!confirmed) return;

            await _useCase.ExecuteReplayAsync(_selectedNode);

            _sceneNavigator.NavigateToAsync(SceneKey.Main).Forget();
        }

        private void HandlePopupCloseRequested()
        {
            _selectedNode = null;
            _view.PopupView.Hide();
        }

        private string GetFrameKeyForState(NodeState state)
        {
            switch (state)
            {
                case NodeState.Selectable: return _frameSpriteKeys[0];
                case NodeState.Locked:     return _frameSpriteKeys[1];
                case NodeState.Hidden:     return _frameSpriteKeys[2];
                default:
                    throw new InvalidOperationException(
                        $"{_logClass} GetFrameKeyForState: unexpected NodeState [{state}]");
            }
        }

        private string GetIconKeyForState(NodeState state, string originalIconKey)
        {
            return state == NodeState.Hidden ? _frameSpriteKeys[3] : originalIconKey;
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

            if (_view?.PopupView != null)
            {
                _view.PopupView.OnRestartClicked -= HandleRestartRequested;
                _view.PopupView.OnCloseClicked   -= HandlePopupCloseRequested;
            }
        }
    }
}
