using System;
using Cysharp.Threading.Tasks;
using Core.ErrorHandling;

namespace App.Systems.ErrorHandling
{
    /// <summary>
    /// One-shot CRITICAL guard, network retry counter, escalation after 4th failure (FR-05, FR-06, FR-09).
    /// Must implement IDisposable; dependencies only via constructor (no GameContext, no UnityEngine).
    /// </summary>
    public class ErrorRecoveryFlow : IDisposable
    {
        private readonly IStabilityFlag _stabilityFlag;
        private readonly IErrorReportSink _reportSink;
        private readonly IPopupService _popupService;
        private readonly ITitleNavigationService _titleNavigation;

        private bool _criticalHandled;
        private int _networkRetryCount;

        public ErrorRecoveryFlow(
            IStabilityFlag stabilityFlag,
            IErrorReportSink reportSink,
            IPopupService popupService,
            ITitleNavigationService titleNavigation)
        {
            _stabilityFlag = stabilityFlag ?? throw new ArgumentNullException(nameof(stabilityFlag));
            _reportSink = reportSink ?? throw new ArgumentNullException(nameof(reportSink));
            _popupService = popupService ?? throw new ArgumentNullException(nameof(popupService));
            _titleNavigation = titleNavigation ?? throw new ArgumentNullException(nameof(titleNavigation));
        }

        public async UniTask HandleCriticalAsync(ErrorReport report, StateSnapshot snapshot)
        {
            if (_criticalHandled)
                return;

            _criticalHandled = true;
            _stabilityFlag.Block();
            _reportSink.Report(report);

            bool choseFirst = await _popupService.ShowCommonPopup(
                title: "Error",
                desc: report?.Message ?? "A critical error occurred.",
                firstText: "To Title",
                secondText: "");

            if (choseFirst)
                _titleNavigation.NavigateToTitle();
        }

        public async UniTask HandleNetworkAsync(ErrorReport report, NetworkRetryCommand retryCommand)
        {
            bool choseReconnect = await _popupService.ShowCommonPopup(
                title: "Connection Error",
                desc: report?.Message ?? "Network error. Reconnect or return to title.",
                firstText: "Reconnect",
                secondText: "To Title");

            if (choseReconnect && retryCommand != null)
            {
                await retryCommand.Invoke();
                _networkRetryCount++;
                if (_networkRetryCount >= 4)
                {
                    var criticalReport = new ErrorReport
                    {
                        Severity = ErrorSeverity.CRITICAL,
                        Message = report?.Message ?? "Network escalation after retries",
                        StackTrace = report?.StackTrace,
                        SnapshotId = report?.SnapshotId,
                        TimestampUtc = report?.TimestampUtc ?? 0,
                        ContextScene = report?.ContextScene,
                        ContextFrame = report?.ContextFrame ?? 0
                    };
                    await HandleCriticalAsync(criticalReport, null);
                }
            }
            else if (!choseReconnect)
            {
                _titleNavigation.NavigateToTitle();
            }
        }

        public void Dispose() { }
    }
}
