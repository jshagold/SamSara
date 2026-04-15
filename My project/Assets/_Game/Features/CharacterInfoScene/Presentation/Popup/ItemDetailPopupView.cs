using Samsara.Features.Character.MasterData;
using Samsara.Features.Inventory.Domain;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Samsara.Features.CharacterInfoScene.Presentation.Popup
{
    /// <summary>
    /// 인벤토리 아이템 상세 팝업 View. 사용/버리기/닫기 이벤트를 Presenter로 전달한다.
    /// </summary>
    public class ItemDetailPopupView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(ItemDetailPopupView)}]";

        [SerializeField] private GameObject _popupRoot;
        [SerializeField] private Image      _iconImage;
        [SerializeField] private TMP_Text   _itemNameText;
        [SerializeField] private TMP_Text   _descriptionText;
        [SerializeField] private TMP_Text   _effectText;
        [SerializeField] private TMP_Text   _quantityText;
        [SerializeField] private Button     _useButton;
        [SerializeField] private Button     _discardButton;
        [SerializeField] private Button     _closeButton;

        public event System.Action OnUseClicked;
        public event System.Action OnDiscardClicked;
        public event System.Action OnCloseClicked;

        private void Awake()
        {
            _useButton.onClick.AddListener(()     => OnUseClicked?.Invoke());
            _discardButton.onClick.AddListener(() => OnDiscardClicked?.Invoke());
            _closeButton.onClick.AddListener(()   => OnCloseClicked?.Invoke());
        }

        public void Show(ItemDetailData data)
        {
            _iconImage.sprite     = null; // Phase 1: ISpriteLoader 미주입
            _itemNameText.text    = data.ItemName;
            _descriptionText.text = data.Description;
            _quantityText.text    = $"보유: {data.Quantity}";

            string statName = data.TargetStat switch
            {
                StatType.Hp        => "HP",
                StatType.Strength  => "공격력",
                StatType.Toughness => "방어력",
                StatType.Agility   => "민첩",
                _                  => data.TargetStat.ToString()
            };
            _effectText.text = $"{statName} +{data.EffectValue}";

            _popupRoot.SetActive(true);
        }

        public void Hide()
        {
            _popupRoot.SetActive(false);
        }

        private void Reset()
        {
            _closeButton = GetComponentInChildren<Button>();
            // _useButton, _discardButton, TMP_Text 4개는 순서 모호성으로 Inspector에서 수동 배정
        }

        private void OnDestroy()
        {
            _useButton?.onClick.RemoveAllListeners();
            _discardButton?.onClick.RemoveAllListeners();
            _closeButton?.onClick.RemoveAllListeners();
        }
    }
}
