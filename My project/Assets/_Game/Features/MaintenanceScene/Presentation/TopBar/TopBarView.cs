using System;
using UnityEngine;

namespace Samsara.Features.MaintenanceScene.Presentation.TopBar
{
    public class TopBarView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(TopBarView)}]";

        [SerializeField] private BackButtonView _backButtonView;
        [SerializeField] private OptionButtonView _optionButtonView;

        public event Action OnBackClicked;
        public event Action OnOptionClicked;

        private void Awake()
        {
            _backButtonView.OnBackClicked      += () => OnBackClicked?.Invoke();
            _optionButtonView.OnOptionClicked  += () => OnOptionClicked?.Invoke();
        }

        private void Reset()
        {
            _backButtonView   = GetComponentInChildren<BackButtonView>();
            _optionButtonView = GetComponentInChildren<OptionButtonView>();
        }
    }
}
