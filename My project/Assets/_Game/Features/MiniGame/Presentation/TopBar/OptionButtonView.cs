using System;
using UnityEngine;
using UnityEngine.UI;

namespace Samsara.Features.MiniGame.Presentation.TopBar
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

        private void OnDestroy()
        {
            _button?.onClick.RemoveAllListeners();
        }

        private void Reset()
        {
            _button = GetComponent<Button>();
        }
    }
}
