using System;
using Cysharp.Threading.Tasks;
using Core.ErrorHandling;
using UnityEngine;

namespace App.Systems.ErrorHandling
{
    /// <summary>
    /// App/Systems lifecycle hook: registers log and unhandled exception; routes Error/Exception to classify → snapshot → recovery (FR-01, FR-03).
    /// </summary>
    public class GlobalErrorInterceptor : MonoBehaviour
    {
        private LogRingBuffer _ringBuffer;
        private ErrorClassifier _classifier;
        private ErrorSnapshotCapture _snapshotCapture;
        private ErrorRecoveryFlow _recoveryFlow;
        private IErrorReportSink _reportSink;

        private Application.LogCallback _logCallback;
        private UnhandledExceptionEventHandler _unhandledHandler;
        private bool _handling;

        public void Initialize(
            LogRingBuffer ringBuffer,
            ErrorClassifier classifier,
            ErrorSnapshotCapture snapshotCapture,
            ErrorRecoveryFlow recoveryFlow,
            IErrorReportSink reportSink)
        {
            _ringBuffer = ringBuffer ?? throw new ArgumentNullException(nameof(ringBuffer));
            _classifier = classifier ?? throw new ArgumentNullException(nameof(classifier));
            _snapshotCapture = snapshotCapture ?? throw new ArgumentNullException(nameof(snapshotCapture));
            _recoveryFlow = recoveryFlow ?? throw new ArgumentNullException(nameof(recoveryFlow));
            _reportSink = reportSink ?? throw new ArgumentNullException(nameof(reportSink));
            _logCallback = OnLogMessageReceived;
            _unhandledHandler = OnUnhandledException;
        }

        private void OnEnable()
        {
            if (_logCallback == null) return;
            Application.logMessageReceived += _logCallback;
            AppDomain.CurrentDomain.UnhandledException += _unhandledHandler;
        }

        private void OnDisable()
        {
            if (_logCallback != null)
            {
                Application.logMessageReceived -= _logCallback;
                AppDomain.CurrentDomain.UnhandledException -= _unhandledHandler;
            }
        }

        private void OnLogMessageReceived(string condition, string stackTrace, LogType type)
        {
            int frame = Time.frameCount;
            if (type == LogType.Log || type == LogType.Warning)
            {
                _ringBuffer?.Append(type, condition ?? "", frame);
                return;
            }

            if (type != LogType.Error && type != LogType.Exception)
                return;
            if (_handling) return;
            _handling = true;

            HandleErrorAsync(condition, stackTrace, type).Forget();
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e?.ExceptionObject as Exception;
            if (ex == null) return;
            if (_handling) return;
            _handling = true;

            string condition = ex.Message;
            string stackTrace = ex.StackTrace ?? "";
            _ringBuffer?.Append(LogType.Exception, condition, Time.frameCount);
            HandleErrorAsync(condition, stackTrace, LogType.Exception, ex).Forget();
        }

        private async UniTaskVoid HandleErrorAsync(string condition, string stackTrace, LogType type, Exception exception = null)
        {
            try
            {
                ErrorSeverity severity = type == LogType.Exception && exception != null
                    ? _classifier.Classify(exception, null)
                    : _classifier.Classify(exception ?? new Exception(condition), null);

                StateSnapshot snapshot = await _snapshotCapture.CaptureAsync();
                var report = new ErrorReport
                {
                    Severity = severity,
                    Message = condition ?? "",
                    StackTrace = stackTrace ?? "",
                    SnapshotId = snapshot.SnapshotId,
                    TimestampUtc = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    ContextScene = snapshot.SceneName,
                    ContextFrame = snapshot.FrameCount
                };
                _reportSink.Report(report);

                if (severity == ErrorSeverity.CRITICAL)
                {
                    await _recoveryFlow.HandleCriticalAsync(report, snapshot);
                }
                else if (severity == ErrorSeverity.NETWORK)
                {
                    var retryCommand = new NetworkRetryCommand(() => UniTask.CompletedTask);
                    await _recoveryFlow.HandleNetworkAsync(report, retryCommand);
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception e)
            {
                Debug.LogError($"[GlobalErrorInterceptor] HandleErrorAsync failed: {e}");
            }
            finally
            {
                _handling = false;
            }
        }
    }
}
