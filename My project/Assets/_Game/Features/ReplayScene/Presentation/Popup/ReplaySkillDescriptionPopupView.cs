using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Samsara.Features.ReplayScene.Presentation.Popup
{
    public struct ReplaySkillDescriptionData
    {
        public Sprite Icon;
        public string SkillName;
        public string Description;
        public float  Damage;
    }

    public class ReplaySkillDescriptionPopupView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(ReplaySkillDescriptionPopupView)}]";

        [SerializeField] private GameObject _popupRoot;
        [SerializeField] private Image _dimBackground;
        [SerializeField] private Button _dimButton;
        [SerializeField] private Image _skillIconImage;
        [SerializeField] private TMP_Text _skillNameText;
        [SerializeField] private TMP_Text _skillDescriptionText;
        [SerializeField] private TMP_Text _damageText;
        [SerializeField] private Button _closeButton;

        public event Action OnCloseClicked;

        public void Show(ReplaySkillDescriptionData data)
        {
            _popupRoot.SetActive(true);
            _skillIconImage.sprite = data.Icon;
            _skillNameText.text = data.SkillName;
            _skillDescriptionText.text = data.Description;
            // UI text hardcoded: tracked in Project Backlog GBL-001. Subject to Phase 7 global cleanup.
            _damageText.text = $"데미지: {data.Damage:F1}";
        }

        public void Hide()
        {
            _popupRoot.SetActive(false);
        }

        private void Awake()
        {
            _closeButton.onClick.AddListener(HandleCloseClicked);
            _dimButton.onClick.AddListener(HandleCloseClicked);
            _popupRoot.SetActive(false);
        }

        private void OnDestroy()
        {
            _closeButton?.onClick.RemoveListener(HandleCloseClicked);
            _dimButton?.onClick.RemoveListener(HandleCloseClicked);
        }

        private void HandleCloseClicked()
        {
            OnCloseClicked?.Invoke();
        }

        private void Reset()
        {
            _popupRoot = gameObject;
        }
    }
}
