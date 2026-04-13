using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Samsara.Core.Navigation;
using Samsara.Core.Popup;
using Samsara.Features.BattleScene.Domain;
using Samsara.Features.Event.Domain;
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
        private readonly GameContext _gameContext;

        public StagePresenter(
            StageSceneUseCase useCase,
            StageView view,
            ISceneNavigator sceneNavigator,
            IPopupManager popupManager,
            GameContext gameContext)
        {
            _useCase       = useCase;
            _view          = view;
            _sceneNavigator = sceneNavigator;
            _popupManager  = popupManager;
            _gameContext    = gameContext;
        }

        public void Initialize()
        {
            // Handle battle result from returning BattleScene
            HandleBattleResultIfAny().Forget();

            // Handle event completion from returning EventScene
            HandleEventResultIfAny().Forget();

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

            var nodeType = _useCase.GetNodeType(index);

            // Battle/Event 노드: 씬 전환 전에 노드 완료 처리하지 않음
            // 전투 결과 확인 후 StageScene 복귀 시 처리
            if (nodeType == NodeType.Battle || nodeType == NodeType.Boss)
            {
                var nodeWorldPos = _view.GetNodeWorldPosition(index);
                await _view.MoveCharacterTo(nodeWorldPos, 0.3f);

                var nodes = _useCase.GetCurrentStageNodes();
                var battleData = nodes[index].BattleData;
                _gameContext.PendingBattleContext = new PendingBattleContext(battleData);

                await _sceneNavigator.NavigateToAsync(SceneKey.Battle);
                return;
            }

            if (nodeType == NodeType.Event)
            {
                var nodeWorldPos = _view.GetNodeWorldPosition(index);
                await _view.MoveCharacterTo(nodeWorldPos, 0.3f);

                var eventNodes = _useCase.GetCurrentStageNodes();
                var eventData  = eventNodes[index].EventData;

                if (eventData == null)
                {
                    Debug.LogError($"{_logClass} EventData가 null입니다. 노드 인덱스: {index}");
                    return;
                }

                _gameContext.PendingEventContext = new PendingEventContext
                {
                    EventId     = eventData.EventId,
                    ReturnScene = SceneKey.Stage
                };

                await _sceneNavigator.NavigateToAsync(SceneKey.Event);
                return;
            }

            // 그 외 노드: 즉시 완료 처리
            await _useCase.MoveToNode(index);

            var updatedVm = _useCase.GetStageSceneViewModel();
            _view.SetDay(updatedVm.Day);

            var worldPos = _view.GetNodeWorldPosition(index);
            await _view.MoveCharacterTo(worldPos, 0.3f);

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
        }

        private async UniTaskVoid HandleEventResultIfAny()
        {
            var ctx = _gameContext.PendingEventContext;
            if (ctx == null || !ctx.IsCompleted) return;

            _gameContext.PendingEventContext = null;  // 소비

            var vm = _useCase.GetStageSceneViewModel();
            int eventNodeIndex = vm.CurrentNodeIndex + 1;

            await _useCase.MoveToNode(eventNodeIndex);

            Debug.Log($"{_logClass} 이벤트 완료 — 노드 {eventNodeIndex} 완료 처리.");
        }

        private async UniTaskVoid HandleBattleResultIfAny()
        {
            var result = _gameContext.LastBattleResult;
            if (result == null) return;

            _gameContext.LastBattleResult = null;

            if (result.Value == BattleResult.Victory)
            {
                var vm = _useCase.GetStageSceneViewModel();
                int battleNodeIndex = vm.CurrentNodeIndex + 1;

                await _useCase.MoveToNode(battleNodeIndex);

                Debug.Log($"{_logClass} 전투 승리 — 노드 {battleNodeIndex} 완료 처리.");
            }
            else
            {
                // Defeat: 노드 완료하지 않음 — 현재 위치 유지
                Debug.Log($"{_logClass} 전투 패배 — 노드 미완료, 현재 위치 유지.");
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
