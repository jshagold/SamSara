using System;
using System.Text;
using Cysharp.Threading.Tasks;
using Samsara.Core.Navigation;
using Samsara.Core.Popup;
using Samsara.Features.Character.MasterData;
using Samsara.Features.EvolutionTreeScene.Domain;
using Samsara.Features.EvolutionTreeScene.Presentation.Popup;
using Samsara.Features.EvolutionTreeScene.Presentation.TreeArea;
using UnityEngine;

namespace Samsara.Features.EvolutionTreeScene.Presentation
{
    public class EvolutionTreePresenter : IDisposable
    {
        private readonly string _logClass = $"[{nameof(EvolutionTreePresenter)}]";

        private readonly EvolutionTreeUseCase _useCase;
        private readonly TreeLayoutCalculator _layoutCalculator;
        private readonly EvolutionTreeView _view;
        private readonly ISceneNavigator _sceneNavigator;
        private readonly IPopupManager _popupManager;

        private EvolutionNodeSO[] _allNodes;
        private EvolutionNodeSO _currentNode;
        private EvolutionNodeSO _selectedNode;

        public EvolutionTreePresenter(
            EvolutionTreeUseCase useCase,
            TreeLayoutCalculator layoutCalculator,
            EvolutionTreeView view,
            ISceneNavigator sceneNavigator,
            IPopupManager popupManager)
        {
            _useCase = useCase;
            _layoutCalculator = layoutCalculator;
            _view = view;
            _sceneNavigator = sceneNavigator;
            _popupManager = popupManager;
        }

        public void Initialize()
        {
            _allNodes = _useCase.GetAllNodes();
            _currentNode = _useCase.FindCurrentNode();

            var layoutResult = _layoutCalculator.CalculateLayout(_allNodes);
            _view.TreeScrollView.BuildTree(layoutResult);

            // Setup each node view
            foreach (var node in _allNodes)
            {
                var nodeView = _view.TreeScrollView.GetNodeView(node.NodeId);
                if (nodeView == null) continue;

                nodeView.Setup(node.NodeId, node.NodeIconSprite);
                var state = _useCase.ClassifyNodeState(node, _currentNode);
                nodeView.SetNodeState(state);
            }

            _view.TreeScrollView.ScrollToNode(_currentNode.NodeId);

            // Subscribe events
            var allNodeViews = _view.TreeScrollView.GetAllNodeViews();
            foreach (var nodeView in allNodeViews)
            {
                nodeView.OnNodeClicked += HandleNodeClicked;
            }

            _view.NodeDescriptionPopupView.OnEvolveClicked += HandleEvolveClicked;
            _view.NodeDescriptionPopupView.OnCloseClicked += HandleCloseClicked;
            _view.ReincarnationButtonView.OnReincarnationClicked += HandleReincarnationClicked;
            _view.BackButtonView.OnBackClicked += HandleBackClicked;
        }

        private void HandleNodeClicked(string nodeId)
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
            bool isHiddenLocked = state == NodeState.Hidden;

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

            var data = new NodeDescriptionData
            {
                NodeIcon = null,
                CharacterName = tappedNode.CharacterName,
                StatsText = statsBuilder.ToString(),
                ConditionsText = conditionsBuilder.ToString(),
                SkillIcons = null,
                CanEvolve = state == NodeState.Evolvable,
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
                _view.NodeDescriptionPopupView.OnCloseClicked -= HandleCloseClicked;
            }

            if (_view?.ReincarnationButtonView != null)
                _view.ReincarnationButtonView.OnReincarnationClicked -= HandleReincarnationClicked;

            if (_view?.BackButtonView != null)
                _view.BackButtonView.OnBackClicked -= HandleBackClicked;
        }
    }
}
