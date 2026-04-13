using Cysharp.Threading.Tasks;
using Samsara.Core.Navigation;
using Samsara.Features.Event.Domain;
using Samsara.Features.Event.MasterData;
using UnityEngine;

namespace Samsara.Features.Event.Presentation
{
    public class EventPresenter
    {
        private readonly string _logClass = $"[{nameof(EventPresenter)}]";

        private readonly EventUseCase        _useCase;
        private readonly EventView           _view;
        private readonly ISceneNavigator     _sceneNavigator;
        private readonly PendingEventContext _pendingEventContext;

        private UniTaskCompletionSource _tapTcs;

        public EventPresenter(
            EventUseCase        useCase,
            EventView           view,
            ISceneNavigator     sceneNavigator,
            PendingEventContext pendingEventContext)
        {
            _useCase            = useCase;
            _view               = view;
            _sceneNavigator     = sceneNavigator;
            _pendingEventContext = pendingEventContext;
        }

        // ──────────────────────────────────────────────
        // Initialize
        // ──────────────────────────────────────────────

        public async UniTask InitializeAsync()
        {
            _useCase.LoadEvent(_pendingEventContext.EventId);

            // 배경 설정
            if (!string.IsNullOrEmpty(_pendingEventContext.BackgroundSpriteKey))
            {
                var bg = Resources.Load<Sprite>(_pendingEventContext.BackgroundSpriteKey);
                _view.BackgroundView.SetBackground(bg);
            }

            // 체인 인디케이터
            var chainInfo = _useCase.GetChainInfo();
            if (chainInfo.HasValue)
                _view.ChainStageIndicatorView.Show(chainInfo.Value.eventName, chainInfo.Value.step, chainInfo.Value.total);
            else
                _view.ChainStageIndicatorView.Hide();

            // 전투 복귀 시 — 결과 팝업만 표시
            if (_pendingEventContext.IsReturningFromBattle)
            {
                _pendingEventContext.IsReturningFromBattle = false;
                var applied = _useCase.GetAppliedResult();
                if (applied != null)
                    await ShowResultPopupAsync(applied);
                else
                    await _sceneNavigator.NavigateToAsync(_pendingEventContext.ReturnScene);
                return;
            }

            // 대사 → 선택지/결과 순서 진행
            if (_useCase.GetCurrentDialogue() != null)
                await ShowDialoguePhaseAsync();
            else
                await ShowChoiceOrResultPhaseAsync();
        }

        // ──────────────────────────────────────────────
        // Dialogue Phase
        // ──────────────────────────────────────────────

        private async UniTask ShowDialoguePhaseAsync()
        {
            _view.OnScreenTapped += HandleScreenTap;

            ShowCurrentDialogue();

            while (true)
            {
                _tapTcs = new UniTaskCompletionSource();
                await _tapTcs.Task;

                bool hasNext = _useCase.AdvanceDialogue();
                if (!hasNext) break;
                ShowCurrentDialogue();
            }

            _view.OnScreenTapped -= HandleScreenTap;
            _view.DialogueView.HideDialogue();

            await ShowChoiceOrResultPhaseAsync();
        }

        private void HandleScreenTap()
        {
            _tapTcs?.TrySetResult();
        }

        private void ShowCurrentDialogue()
        {
            var dialogue = _useCase.GetCurrentDialogue();
            if (dialogue != null)
                _view.DialogueView.ShowDialogue(dialogue);
        }

        // ──────────────────────────────────────────────
        // Choice / Result Phase
        // ──────────────────────────────────────────────

        private async UniTask ShowChoiceOrResultPhaseAsync()
        {
            EventResult result;

            if (_useCase.HasChoices())
            {
                var selectionTcs = new UniTaskCompletionSource<int>();
                _view.ChoiceListView.ShowChoices(
                    _useCase.GetChoices(),
                    index => selectionTcs.TrySetResult(index));

                int chosen = await selectionTcs.Task;
                _view.ChoiceListView.HideChoices();
                result = _useCase.ApplyChoice(chosen);
            }
            else
            {
                result = _useCase.ApplyDirectResult();
            }

            await ShowResultPopupAsync(result);
        }

        // ──────────────────────────────────────────────
        // Result Popup
        // ──────────────────────────────────────────────

        private async UniTask ShowResultPopupAsync(EventResult result)
        {
            var text = ComposeResultText(result);
            await _view.EventResultPopupView.ShowAsync(text);
            await HandlePostResultAsync(result);
        }

        private string ComposeResultText(EventResult result)
        {
            switch (result.ResultType)
            {
                case EventResultType.HpChange:
                    return result.Value >= 0
                        ? $"HP +{(int)result.Value}"
                        : $"HP {(int)result.Value}";

                case EventResultType.StatChange:
                    if (result.StatType.HasValue)
                        return $"{result.StatType.Value} {(result.Value >= 0 ? "+" : "")}{(int)result.Value}";
                    return "스탯 변화";

                case EventResultType.ShopEncounter: return "상인을 만났다.";
                case EventResultType.Battle:        return "전투가 시작된다!";
                case EventResultType.Death:         return "쓰러졌다...";
                case EventResultType.None:          return "아무 일도 일어나지 않았다.";
                default:                            return string.Empty;
            }
        }

        // ──────────────────────────────────────────────
        // Post-Result Scene Transition
        // ──────────────────────────────────────────────

        private async UniTask HandlePostResultAsync(EventResult result)
        {
            switch (result.ResultType)
            {
                case EventResultType.Death:
                    await _sceneNavigator.NavigateToAsync(SceneKey.GameOver);
                    break;

                case EventResultType.Battle:
                    // [SPEC-GAP] 이벤트 전투용 BattleNodeData 정의 미비 — ReturnScene으로 복귀 (Stub)
                    // 추후 Patch에서 PendingBattleContext 조립 로직 추가 필요
                    Debug.LogWarning($"{_logClass} Battle result stub: BattleNodeData 미정의. ReturnScene으로 이동.");
                    _pendingEventContext.IsCompleted = true;
                    await _sceneNavigator.NavigateToAsync(_pendingEventContext.ReturnScene);
                    break;

                default:
                    // None, HpChange, StatChange, ShopEncounter
                    _pendingEventContext.IsCompleted = true;
                    await _sceneNavigator.NavigateToAsync(_pendingEventContext.ReturnScene);
                    break;
            }
        }

        // ──────────────────────────────────────────────
        // Dispose
        // ──────────────────────────────────────────────

        public void Dispose()
        {
            if (_view != null)
                _view.OnScreenTapped -= HandleScreenTap;
            _tapTcs?.TrySetCanceled();
        }
    }
}
