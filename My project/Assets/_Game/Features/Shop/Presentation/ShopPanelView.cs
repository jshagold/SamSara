using System;
using System.Collections.Generic;
using Samsara.Features.Shop.Domain;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Samsara.Features.Shop.Presentation
{
    public class ShopPanelView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(ShopPanelView)}]";

        [SerializeField] private Transform        _shopItemContainer;
        [SerializeField] private TMP_Text         _goldText;
        [SerializeField] private Button           _closeButton;
        [SerializeField] private Image            _merchantSpriteImage;
        [SerializeField] private ShopItemSlotView _shopItemSlotPrefab;  // Manual assignment required

        private readonly List<ShopItemSlotView> _slotViews = new List<ShopItemSlotView>();

        public event Action OnCloseClicked;
        public event Action<int> OnPurchaseRequested;

        private void Awake()
        {
            _closeButton.onClick.AddListener(() => OnCloseClicked?.Invoke());
        }

        public void Show(List<ShopItemInfo> items, int gold)
        {
            ClearSlots();

            foreach (var item in items)
            {
                var slot = Instantiate(_shopItemSlotPrefab, _shopItemContainer);
                slot.Setup(item, potionId => OnPurchaseRequested?.Invoke(potionId));
                _slotViews.Add(slot);
            }

            UpdateGold(gold);
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            ClearSlots();
            gameObject.SetActive(false);
        }

        public void UpdateGold(int gold)
        {
            _goldText.text = $"{gold} G";
        }

        /// <summary>특정 슬롯의 재고 수를 갱신한다. slotIndex는 items 리스트 순서와 동일.</summary>
        public void UpdateSlot(int slotIndex, int remainingStock)
        {
            if (slotIndex < 0 || slotIndex >= _slotViews.Count) return;
            _slotViews[slotIndex].UpdateStock(remainingStock);
        }

        public void SetMerchantSprite(Sprite sprite)
        {
            if (sprite != null)
                _merchantSpriteImage.sprite = sprite;
        }

        private void ClearSlots()
        {
            foreach (var slot in _slotViews)
            {
                if (slot != null) Destroy(slot.gameObject);
            }
            _slotViews.Clear();
        }

        private void OnDestroy()
        {
            _closeButton?.onClick.RemoveAllListeners();
        }

        private void Reset()
        {
            _shopItemContainer = transform;
            _goldText          = GetComponentInChildren<TMP_Text>();
            _closeButton       = GetComponentInChildren<Button>();
            // _shopItemSlotPrefab: Inspector에서 수동 연결 필요
        }
    }
}
