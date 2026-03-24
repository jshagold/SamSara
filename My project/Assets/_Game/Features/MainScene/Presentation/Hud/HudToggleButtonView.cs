using System;
using UnityEngine;
using UnityEngine.UI;

namespace Samsara.Features.MainScene.Presentation.Hud
{
    public class HudToggleButtonView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(HudToggleButtonView)}]";

        [SerializeField] private Button _toggleButton;

        public event Action OnToggleClicked;

        private void Awake()
        {
            _toggleButton.onClick.AddListener(() => OnToggleClicked?.Invoke());
        }

        private void OnDestroy()
        {
            _toggleButton?.onClick.RemoveAllListeners();
        }

        private void Reset()
        {
            _toggleButton = GetComponentInChildren<Button>();
        }
    }
}
