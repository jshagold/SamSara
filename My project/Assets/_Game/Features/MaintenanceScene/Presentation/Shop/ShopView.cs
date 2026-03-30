using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Samsara.Features.MaintenanceScene.Presentation.Shop
{
    public class ShopView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(ShopView)}]";

        [SerializeField] private Button _closeButton;
        [SerializeField] private TMP_Text _placeholderText;

        public event Action OnCloseClicked;

        private void Awake()
        {
            _closeButton.onClick.AddListener(() => OnCloseClicked?.Invoke());
        }

        public void Show()
        {
            // TODO: Phase 5 — 포션 목록, 구매 로직, 골드 표시 구현.
            _placeholderText.text = "Shop coming soon";
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            _closeButton?.onClick.RemoveAllListeners();
        }

        private void Reset()
        {
            _closeButton     = GetComponentInChildren<Button>();
            _placeholderText = GetComponentInChildren<TMP_Text>();
        }
    }
}
