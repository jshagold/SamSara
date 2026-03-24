using System;
using UnityEngine;
using UnityEngine.UI;

namespace Samsara.Features.MainScene.Presentation.Main
{
    public class MerchantButtonView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(MerchantButtonView)}]";

        [SerializeField] private Button _button;

        public event Action OnMerchantClicked;

        public void SetVisible(bool visible)
        {
            gameObject.SetActive(visible);
        }

        private void Awake()
        {
            _button.onClick.AddListener(() => OnMerchantClicked?.Invoke());
        }

        private void OnDestroy()
        {
            _button?.onClick.RemoveAllListeners();
        }

        private void Reset()
        {
            _button = GetComponentInChildren<Button>();
        }
    }
}
