using System;
using UnityEngine;
using UnityEngine.UI;

namespace Samsara.Features.CharacterInfoScene.Presentation.TopBar
{
    public class BackButtonView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(BackButtonView)}]";

        [SerializeField] private Button _button;

        public event Action OnBackClicked;

        private void Awake()
        {
            _button.onClick.AddListener(() => OnBackClicked?.Invoke());
        }

        private void Reset()
        {
            _button = GetComponentInChildren<Button>();
        }
    }
}
