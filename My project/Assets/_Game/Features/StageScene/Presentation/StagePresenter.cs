using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Samsara.Core.Navigation;
using Samsara.Core.Popup;
using Samsara.Features.Stage.MasterData;
using Samsara.Features.StageScene.Domain;
using Samsara.Features.StageScene.Presentation.Popup;
using UnityEngine;

namespace Samsara.Features.StageScene.Presentation
{
    public class StagePresenter
    {
        private readonly string _logClass = $"[{nameof(StagePresenter)}]";

        private readonly StageSceneUseCase _useCase;
        private readonly StageView _view;
        private readonly ISceneNavigator _sceneNavigator;
        private readonly IPopupManager _popupManager;

        public StagePresenter(
            StageSceneUseCase useCase,
            StageView view,
            ISceneNavigator sceneNavigator,
            IPopupManager popupManager)
        {
            _useCase       = useCase;
            _view          = view;
            _sceneNavigator = sceneNavigator;
            _popupManager  = popupManager;
        }

        public void Initialize()
        {
            var vm    = _useCase.GetStageSceneViewModel();
            var nodes = _useCase.GetCurrentStageNodes();

            _view.RenderNodes(nodes);

            foreach (var completedIndex in vm.CompletedNodeIndices)
                _view.MarkNodeCompleted(completedIndex);

            _view.HighlightNode(vm.CurrentNodeIndex);
            _view.FocusOnNode(vm.CurrentNodeIndex);
            _view.SetCharacterPosition(_view.GetNodeWorldPosition(vm.CurrentNodeIndex));

            // TODO: [BACKLOG] Load background Sprite from BiomeSpriteKey via Addressables.
            _view.SetBackground(null);

            _view.SetDay(vm.Day);
            _view.SetBackButtonInteractable(vm.CanReturnToMain);

            _view.OnNodeClicked       += HandleNodeClicked;
            _view.OnBackClicked       += HandleBackClicked;
            _view.OnOptionClicked     += HandleOptionClicked;
            _view.OnStageSelected     += HandleStageSelected;
            _view.OnReturnToMainClicked += HandleReturnToMain;

            Debug.Log($"{_logClass} Initialize 완료. CurrentNode={vm.CurrentNodeIndex}, Day={vm.Day}");
        }

        public void Dispose()
        {
            _view.OnNodeClicked       -= HandleNodeClicked;
            _view.OnBackClicked       -= HandleBackClicked;
            _view.OnOptionClicked     -= HandleOptionClicked;
            _view.OnStageSelected     -= HandleStageSelected;
            _view.OnReturnToMainClicked -= HandleReturnToMain;
        }

        // ── Event handlers ──────────────────────────────────────────────

        private void HandleNodeClicked(int index)    => HandleNodeClickedAsync(index).Forget();
        private void HandleBackClicked()             => HandleBackClickedAsync().Forget();
        private void HandleOptionClicked()           { /* TODO: Show option/pause menu */ }
        private void HandleStageSelected(string id) => HandleStageSelectedAsync(id).Forget();
        private void HandleReturnToMain()            => HandleReturnToMainAsync().Forget();

        // ── Async implementations ───────────────────────────────────────

        private async UniTaskVoid HandleNodeClickedAsync(int index)
        {
            var vm = _useCase.GetStageSceneViewModel();
            if (index != vm.CurrentNodeIndex + 1)
            {
                Debug.Log($"{_logClass} 이동 불가 — 현재:{vm.CurrentNodeIndex}, 요청:{index}");
                return;
            }

            await _useCase.MoveToNode(index);

            var updatedVm = _useCase.GetStageSceneViewModel();
            _view.SetDay(updatedVm.Day);

            var nodeWorldPos = _view.GetNodeWorldPosition(index);
            await _view.MoveCharacterTo(nodeWorldPos, 0.3f);

            _view.MarkNodeCompleted(index - 1);
            _view.HighlightNode(index);
            _view.FocusOnNode(index);
            _view.SetBackButtonInteractable(_useCase.CanReturnToMain());

            if (_useCase.IsStageComplete(index))
            {
                var nextStages = _useCase.GetNextStageOptions();
                var options = new List<StageOptionData>(nextStages.Count);
                foreach (var stage in nextStages)
                    options.Add(new StageOptionData { StageId = stage.StageId, StageName = stage.StageName });
                _view.ShowStageCompletePopup("Stage Complete!", options);
            }
            else
            {
                var nodeType = _useCase.GetNodeType(index);
                switch (nodeType)
                {
                    case NodeType.Battle:
                    case NodeType.Boss:
                        // TODO: [BACKLOG] BattleScene 구현 후 연결
                        await _sceneNavigator.NavigateToAsync(SceneKey.Battle);
                        break;
                    case NodeType.Event:
                        // TODO: [BACKLOG] EventScene 구현 후 연결
                        await _sceneNavigator.NavigateToAsync(SceneKey.ActionEvent);
                        break;
                }
            }
        }

        private async UniTaskVoid HandleBackClickedAsync()
        {
            if (!_useCase.CanReturnToMain()) return;
            await _sceneNavigator.NavigateToAsync(SceneKey.Main);
        }

        private async UniTaskVoid HandleStageSelectedAsync(string stageId)
        {
            await _useCase.SelectNextStage(stageId);
            _view.HideStageCompletePopup();

            var nodes = _useCase.GetCurrentStageNodes();
            _view.ClearNodes();
            _view.RenderNodes(nodes);

            var vm = _useCase.GetStageSceneViewModel();
            _view.HighlightNode(vm.CurrentNodeIndex);
            _view.FocusOnNode(vm.CurrentNodeIndex);
            _view.SetCharacterPosition(_view.GetNodeWorldPosition(vm.CurrentNodeIndex));
            _view.SetBackButtonInteractable(vm.CanReturnToMain);

            // TODO: [BACKLOG] Update background sprite for new stage biome
        }

        private async UniTaskVoid HandleReturnToMainAsync()
        {
            await _sceneNavigator.NavigateToAsync(SceneKey.Main);
        }
    }
}
