using System;
using UnityEngine;
using UnityEngine.UI;

namespace Samsara.Features.EvolutionTreeScene.Presentation.TopBar
{
    public class BackButtonView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(BackButtonView)}]";

        [SerializeField] private Button _button;

        public event Action OnBackClicked;

        private void OnEnable()
        {
            _button.onClick.AddListener(HandleBackClicked);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(HandleBackClicked);
        }

        private void HandleBackClicked()
        {
            OnBackClicked?.Invoke();
        }

        private void Reset()
        {
            _button = GetComponentInChildren<Button>();
        }
    }
}
