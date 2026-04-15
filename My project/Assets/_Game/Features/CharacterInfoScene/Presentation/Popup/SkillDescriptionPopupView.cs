using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Samsara.Features.CharacterInfoScene.Presentation.Popup
{
    public class SkillDescriptionPopupView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(SkillDescriptionPopupView)}]";

        [SerializeField] private GameObject _popupRoot;
        [SerializeField] private Image      _skillIconImage;
        [SerializeField] private TMP_Text   _skillNameText;
        [SerializeField] private TMP_Text   _skillDescriptionText;
        [SerializeField] private TMP_Text   _damageText;
        [SerializeField] private TMP_Text   _effectText;
        [SerializeField] private Button     _closeButton;

        public event Action OnCloseClicked;

        private void Awake()
        {
            _closeButton.onClick.AddListener(() => OnCloseClicked?.Invoke());
            _popupRoot.SetActive(false);
        }

        /// <summary>팝업을 열고 스킬 정보를 표시한다.</summary>
        public void Show(Sprite icon, string skillName, string description, float damage, string effectDescription)
        {
            _skillIconImage.sprite     = icon;
            _skillNameText.text        = skillName;
            _skillDescriptionText.text = description;
            _damageText.text           = $"데미지: {damage:F1}";
            _effectText.text           = effectDescription;
            _popupRoot.SetActive(true);
        }

        /// <summary>팝업을 닫는다.</summary>
        public void Hide()
        {
            _popupRoot.SetActive(false);
        }

        private void Reset()
        {
            _closeButton    = GetComponentInChildren<Button>();
            _skillIconImage = GetComponentInChildren<Image>();
            // TMP_Text 4개는 순서 모호성으로 Inspector에서 수동 배정
        }

        private void OnDestroy()
        {
            _closeButton?.onClick.RemoveAllListeners();
        }
    }
}
