using System;
using UnityEngine;
using UnityEngine.UI;

namespace Samsara.Features.ReplayScene.Presentation
{
    public class OptionButtonView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(OptionButtonView)}]";

        [SerializeField] private Button _button;

        public event Action OnTapped;

        private void Awake()
        {
            _button.onClick.AddListener(HandleClick);
        }

        private void OnDestroy()
        {
            _button?.onClick.RemoveListener(HandleClick);
        }

        private void HandleClick()
        {
            OnTapped?.Invoke();
        }

        private void Reset()
        {
            _button = GetComponentInChildren<Button>();
        }
    }
}
