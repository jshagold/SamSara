using System;
using UnityEngine;
using UnityEngine.UI;

namespace Samsara.Features.MiniGame.Presentation.Play
{
    public class TouchButtonView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(TouchButtonView)}]";

        [SerializeField] private Button _button;

        public event Action OnTouchClicked;

        private void Awake()
        {
            _button.onClick.AddListener(() => OnTouchClicked?.Invoke());
        }

        public void SetInteractable(bool interactable)
        {
            _button.interactable = interactable;
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
