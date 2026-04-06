using System;
using UnityEngine;
using UnityEngine.UI;

namespace Samsara.Features.CharacterInfoScene.Presentation.TopBar
{
    public class OptionButtonView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(OptionButtonView)}]";

        [SerializeField] private Button _button;

        public event Action OnOptionClicked;

        private void Awake()
        {
            _button.onClick.AddListener(() => OnOptionClicked?.Invoke());
        }

        private void Reset()
        {
            _button = GetComponentInChildren<Button>();
        }
    }
}
