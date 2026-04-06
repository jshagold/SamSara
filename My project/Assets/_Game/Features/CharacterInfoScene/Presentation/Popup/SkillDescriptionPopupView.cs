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
        }

        public void Show(Sprite icon, string skillName, string description, float damage, string effectDescription)
        {
            _skillIconImage.sprite    = icon;
            _skillNameText.text       = skillName;
            _skillDescriptionText.text = description;
            _damageText.text          = $"Damage: {damage}";
            _effectText.text          = effectDescription;
            _popupRoot.SetActive(true);
        }

        public void Hide()
        {
            _popupRoot.SetActive(false);
        }

        private void Reset()
        {
            _popupRoot    = gameObject;
            _skillIconImage       = GetComponentInChildren<Image>();
            _closeButton          = GetComponentInChildren<Button>();
            _skillNameText        = GetComponentInChildren<TMP_Text>();
        }
    }
}
