using System;
using UnityEngine;
using UnityEngine.UI;

namespace Samsara.Features.BattleScene.Presentation.TopBar
{
    public class OptionButtonView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(OptionButtonView)}]";

        [SerializeField] private Button _optionButton;

        public event Action OnOptionClicked;

        private void Awake()
        {
            _optionButton.onClick.AddListener(() => OnOptionClicked?.Invoke());
        }

        private void OnDestroy()
        {
            _optionButton?.onClick.RemoveAllListeners();
        }

        private void Reset()
        {
            _optionButton = GetComponentInChildren<Button>();
        }
    }
}
