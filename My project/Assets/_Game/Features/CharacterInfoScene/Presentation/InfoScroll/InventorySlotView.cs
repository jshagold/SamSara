using Samsara.Features.Inventory.Domain;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Samsara.Features.CharacterInfoScene.Presentation.InfoScroll
{
    /// <summary>
    /// 인벤토리 슬롯 단위 View. 슬롯 클릭 이벤트를 상위로 전달한다.
    /// </summary>
    public class InventorySlotView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(InventorySlotView)}]";

        [SerializeField] private Button    _button;
        [SerializeField] private Image     _iconImage;
        [SerializeField] private TMP_Text  _quantityText;
        [SerializeField] private GameObject _emptyIndicator;

        public int SlotIndex { get; private set; }

        public event System.Action<int> OnSlotClicked;

        private void Awake()
        {
            _button.onClick.AddListener(() => OnSlotClicked?.Invoke(SlotIndex));
        }

        public void Setup(int slotIndex, InventorySlotDisplayData data)
        {
            SlotIndex = slotIndex;

            bool isEmpty = data.IsEmpty;
            _iconImage.gameObject.SetActive(!isEmpty);
            _quantityText.gameObject.SetActive(!isEmpty);

            if (_emptyIndicator != null)
                _emptyIndicator.SetActive(isEmpty);

            if (!isEmpty)
            {
                _iconImage.sprite  = null; // Phase 1: ISpriteLoader 미주입
                _quantityText.text = data.Quantity > 1 ? data.Quantity.ToString() : string.Empty;
            }
        }

        private void Reset()
        {
            _button       = GetComponentInChildren<Button>();
            _iconImage    = GetComponentInChildren<Image>();
            _quantityText = GetComponentInChildren<TMP_Text>();
        }
    }
}
