using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Samsara.Features.EvolutionTreeScene.Presentation.Popup
{
    public struct NodeDescriptionData
    {
        public Sprite NodeIcon;
        public string CharacterName;
        public string StatsText;
        public string ConditionsText;
        public Sprite[] SkillIcons;
        public bool CanEvolve;
        public bool IsHiddenLocked;
    }

    public class NodeDescriptionPopupView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(NodeDescriptionPopupView)}]";

        [SerializeField] private GameObject _popupRoot;
        [SerializeField] private Image _dimBackground;
        [SerializeField] private Button _dimBackgroundButton;
        [SerializeField] private Image _nodeIconImage;
        [SerializeField] private TMP_Text _characterNameText;
        [SerializeField] private TMP_Text _statsText;
        [SerializeField] private TMP_Text _unlockConditionsText;
        [SerializeField] private Transform _skillIconContainer;
        [SerializeField] private Image[] _skillIcons;
        [SerializeField] private Button _evolveButton;
        [SerializeField] private Button _closeButton;

        public event Action OnEvolveClicked;
        public event Action OnCloseClicked;

        public void Show(NodeDescriptionData data)
        {
            _popupRoot.SetActive(true);

            _nodeIconImage.sprite = data.NodeIcon;

            if (data.IsHiddenLocked)
            {
                _characterNameText.text = "???";
                _statsText.text = "???";
                _unlockConditionsText.text = data.ConditionsText;
                _skillIconContainer.gameObject.SetActive(false);
                _evolveButton.gameObject.SetActive(false);
            }
            else
            {
                _characterNameText.text = data.CharacterName;
                _statsText.text = data.StatsText;
                _unlockConditionsText.text = data.ConditionsText;
                _skillIconContainer.gameObject.SetActive(true);

                for (int i = 0; i < _skillIcons.Length; i++)
                {
                    if (data.SkillIcons != null && i < data.SkillIcons.Length && data.SkillIcons[i] != null)
                    {
                        _skillIcons[i].gameObject.SetActive(true);
                        _skillIcons[i].sprite = data.SkillIcons[i];
                    }
                    else
                    {
                        _skillIcons[i].gameObject.SetActive(false);
                    }
                }

                _evolveButton.gameObject.SetActive(data.CanEvolve);
            }
        }

        public void Hide()
        {
            _popupRoot.SetActive(false);
        }

        private void OnEnable()
        {
            _evolveButton.onClick.AddListener(HandleEvolveClicked);
            _closeButton.onClick.AddListener(HandleCloseClicked);
            _dimBackgroundButton.onClick.AddListener(HandleCloseClicked);
        }

        private void OnDisable()
        {
            _evolveButton.onClick.RemoveListener(HandleEvolveClicked);
            _closeButton.onClick.RemoveListener(HandleCloseClicked);
            _dimBackgroundButton.onClick.RemoveListener(HandleCloseClicked);
        }

        private void HandleEvolveClicked()
        {
            OnEvolveClicked?.Invoke();
        }

        private void HandleCloseClicked()
        {
            OnCloseClicked?.Invoke();
        }

        private void Reset()
        {
            _evolveButton = GetComponentInChildren<Button>();
        }
    }
}
