using System;
using Samsara.Features.Shop.Domain;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Samsara.Features.Shop.Presentation
{
    public class ShopItemSlotView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(ShopItemSlotView)}]";

        [SerializeField] private Image    _potionIcon;
        [SerializeField] private TMP_Text _potionNameText;
        [SerializeField] private TMP_Text _priceText;
        [SerializeField] private TMP_Text _stockText;
        [SerializeField] private Button   _purchaseButton;

        private int _potionId;

        public void Setup(ShopItemInfo itemInfo, Action<int> onPurchase)
        {
            _potionId = itemInfo.Potion.Id;

            _potionNameText.text = itemInfo.Potion.PotionName;
            _priceText.text      = $"{itemInfo.Price} G";
            _stockText.text      = $"x{itemInfo.RemainingStock}";

            _purchaseButton.onClick.RemoveAllListeners();
            _purchaseButton.onClick.AddListener(() => onPurchase(_potionId));
        }

        public void SetIcon(Sprite sprite)
        {
            _potionIcon.sprite = sprite;
        }

        public void UpdateStock(int remainingStock)
        {
            _stockText.text = $"x{remainingStock}";
        }

        private void OnDestroy()
        {
            _purchaseButton?.onClick.RemoveAllListeners();
        }

        private void Reset()
        {
            _potionIcon     = GetComponentInChildren<Image>();
            _potionNameText = GetComponentInChildren<TMP_Text>();
            _priceText      = GetComponentInChildren<TMP_Text>();
            _stockText      = GetComponentInChildren<TMP_Text>();
            _purchaseButton = GetComponentInChildren<Button>();
        }
    }
}
