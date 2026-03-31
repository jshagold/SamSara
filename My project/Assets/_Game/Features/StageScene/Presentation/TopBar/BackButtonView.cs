using System;
using UnityEngine;
using UnityEngine.UI;

namespace Samsara.Features.StageScene.Presentation.TopBar
{
    public class BackButtonView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(BackButtonView)}]";

        [SerializeField] private Button _button;
        [SerializeField] private CanvasGroup _canvasGroup;

        public event Action OnBackClicked;

        private void Awake()
        {
            _button.onClick.AddListener(() => OnBackClicked?.Invoke());
        }

        public void SetInteractable(bool interactable)
        {
            _button.interactable = interactable;
            _canvasGroup.alpha = interactable ? 1f : 0.4f;
        }

        private void Reset()
        {
            _button = GetComponentInChildren<Button>();
            _canvasGroup = GetComponentInChildren<CanvasGroup>();
        }

        private void OnDestroy()
        {
            _button?.onClick.RemoveAllListeners();
        }
    }
}
