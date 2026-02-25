using System;
using App.Systems.ErrorHandling;
using Core.ErrorHandling;

namespace Features.ErrorHandling.Presentation
{
    /// <summary>
    /// Binds ErrorRecoveryFlow to ErrorPopupView; disables buttons during retry (UR-05, M3).
    /// </summary>
    public class ErrorPopupPresenter : IDisposable
    {
        private readonly ErrorPopupView _view;
        private readonly ErrorRecoveryFlow _recoveryFlow;

        public ErrorPopupPresenter(ErrorPopupView view, ErrorRecoveryFlow recoveryFlow)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));
            _recoveryFlow = recoveryFlow ?? throw new ArgumentNullException(nameof(recoveryFlow));
        }

        public void Initialize()
        {
            _recoveryFlow.RetryStarted += OnRetryStarted;
            _recoveryFlow.RetryCompleted += OnRetryCompleted;
            _view.SetButtonsInteractable(true);
        }

        public void Dispose()
        {
            _recoveryFlow.RetryStarted -= OnRetryStarted;
            _recoveryFlow.RetryCompleted -= OnRetryCompleted;
        }

        private void OnRetryStarted()
        {
            _view?.SetButtonsInteractable(false);
        }

        private void OnRetryCompleted()
        {
            _view?.SetButtonsInteractable(true);
        }
    }
}
