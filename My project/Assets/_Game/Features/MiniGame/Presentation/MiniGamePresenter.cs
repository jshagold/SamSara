using System;
using Cysharp.Threading.Tasks;
using Samsara.Core.Navigation;
using Samsara.Features.Character.MasterData;
using Samsara.Features.MiniGame.Domain;
using UnityEngine;

namespace Samsara.Features.MiniGame.Presentation
{
    public class MiniGamePresenter
    {
        private readonly string _logClass = $"[{nameof(MiniGamePresenter)}]";

        private readonly MiniGameUseCase  _useCase;
        private readonly MiniGameView     _view;
        private readonly ISceneNavigator  _sceneNavigator;

        // 타이밍 파라미터 (TBD — 스탯 설계 확정 후 조정 예정)
        private const float MarkerSpeed        = 300f;  // pixels/sec
        private const float SuccessZoneStart   = 0.35f;
        private const float SuccessZoneEnd     = 0.65f;
        private const int   RoundResultDelayMs = 500;

        private static readonly int RoundCount = MiniGameUseCase.RoundCount;

        // 런타임 상태
        private StatType _statType;
        private int      _beforeValue;
        private int      _roundIndex;
        private int      _successCount;

        // 캐시된 핸들러 — Dispose 시 정확한 구독 해제를 위해 보관
        private Action _touchClickedHandler;
        private Action _optionClickedHandler;

        public MiniGamePresenter(
            MiniGameUseCase  useCase,
            MiniGameView     view,
            ISceneNavigator  sceneNavigator)
        {
            _useCase        = useCase;
            _view           = view;
            _sceneNavigator = sceneNavigator;
        }

        public void Initialize(StatType statType)
        {
            _statType     = statType;
            _roundIndex   = 0;
            _successCount = 0;

            _beforeValue = _useCase.GetCurrentStatValue(statType);

            _view.GuideTooltip.SetText(GetGuideText(statType));
            _view.TimingBar.SetSuccessZone(SuccessZoneStart, SuccessZoneEnd);
            _view.RoundIndicator.ResetAll();
            _view.RoundIndicator.SetRound(1, RoundCount);

            _touchClickedHandler  = () => HandleTouchClickedAsync().Forget();
            _optionClickedHandler = HandleOptionClicked;

            _view.TouchButton.OnTouchClicked  += _touchClickedHandler;
            _view.OptionButton.OnOptionClicked += _optionClickedHandler;

            StartRound();

            Debug.Log($"{_logClass} Initialize 완료 — StatType={statType} beforeValue={_beforeValue}");
        }

        public void Dispose()
        {
            _view.TouchButton.OnTouchClicked  -= _touchClickedHandler;
            _view.OptionButton.OnOptionClicked -= _optionClickedHandler;

            Debug.Log($"{_logClass} Dispose 완료.");
        }

        // ── Round Logic ──

        private void StartRound()
        {
            float barWidth = _view.TimingBar.GetBarWidth();
            _view.Marker.ResetPosition();
            _view.Marker.StartOscillation(barWidth, MarkerSpeed);
            _view.TouchButton.SetInteractable(true);
        }

        private async UniTaskVoid HandleTouchClickedAsync()
        {
            _view.TouchButton.SetInteractable(false);
            _view.Marker.StopOscillation();

            float normalizedPos = _view.Marker.GetNormalizedPosition(_view.TimingBar.GetBarWidth());
            bool  isSuccess     = normalizedPos >= SuccessZoneStart && normalizedPos <= SuccessZoneEnd;

            if (isSuccess) _successCount++;

            _view.RoundIndicator.SetRoundResult(_roundIndex, isSuccess);

            Debug.Log($"{_logClass} Round {_roundIndex + 1} — pos={normalizedPos:F2} success={isSuccess}");

            await UniTask.Delay(RoundResultDelayMs);

            _roundIndex++;

            if (_roundIndex >= RoundCount)
            {
                await FinishGame();
            }
            else
            {
                _view.RoundIndicator.SetRound(_roundIndex + 1, RoundCount);
                StartRound();
            }
        }

        private async UniTask FinishGame()
        {
            var verdict = _successCount switch
            {
                3 => MiniGameVerdict.Success,
                2 => MiniGameVerdict.Maintain,
                _ => MiniGameVerdict.Fail
            };

            int delta      = _useCase.CalculateStatDelta(verdict);
            int afterValue = _beforeValue + delta;

            // Save-on-Action: 팝업 표시 전에 저장 완료 (§9)
            await _useCase.ApplyResultAndSave(_statType, delta);

            string statName = GetStatDisplayName(_statType);
            await _view.ResultPopup.Show(verdict, statName, _beforeValue, afterValue);

            await _sceneNavigator.NavigateToAsync(SceneKey.Maintenance);
        }

        private void HandleOptionClicked()
        {
            Debug.Log($"{_logClass} Option 클릭 — 미구현.");
        }

        // ── Helpers ──

        private static string GetGuideText(StatType statType)
        {
            return statType switch
            {
                StatType.Hp        => "HP 훈련 — 타이밍에 맞춰 버튼을 누르세요!",
                StatType.Strength  => "근력 훈련 — 타이밍에 맞춰 버튼을 누르세요!",
                StatType.Toughness => "강인함 훈련 — 타이밍에 맞춰 버튼을 누르세요!",
                StatType.Agility   => "민첩 훈련 — 타이밍에 맞춰 버튼을 누르세요!",
                _                  => "타이밍에 맞춰 버튼을 누르세요!"
            };
        }

        private static string GetStatDisplayName(StatType statType)
        {
            return statType switch
            {
                StatType.Hp        => "HP",
                StatType.Strength  => "근력",
                StatType.Toughness => "강인함",
                StatType.Agility   => "민첩",
                _                  => statType.ToString()
            };
        }
    }
}
