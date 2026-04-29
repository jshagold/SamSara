using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Samsara.Features.ReplayScene.Presentation.Popup
{
    public struct ReplayNodeDescriptionData
    {
        public Sprite   NodeIcon;
        public string   CharacterName;
        public string   StatsText;
        public string   ConditionsText;
        public Sprite[] SkillIcons;
        public bool     CanRestart;
        public bool     IsHiddenLocked;
    }

    public class ReplayNodeDescriptionPopupView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(ReplayNodeDescriptionPopupView)}]";

        [SerializeField] private GameObject _popupRoot;
        [SerializeField] private Image _dimBackground;
        [SerializeField] private Button _dimButton;
        [SerializeField] private Image _nodeIconImage;
        [SerializeField] private TMP_Text _characterNameText;
        [SerializeField] private TMP_Text _statsText;
        [SerializeField] private TMP_Text _conditionsText;
        [SerializeField] private ReplaySkillListView _skillListView;
        [SerializeField] private Button _restartButton;
        [SerializeField] private Button _closeButton;

        public event Action OnRestartClicked;
        public event Action OnCloseClicked;
        public event Action<int> OnSkillSlotClicked;

        public void Show(ReplayNodeDescriptionData data)
        {
            _popupRoot.SetActive(true);
            _nodeIconImage.sprite = data.NodeIcon;

            if (data.IsHiddenLocked)
            {
                // UI text hardcoded: tracked in Project Backlog GBL-001. Subject to Phase 7 global cleanup.
                _characterNameText.text = "???";
                // UI text hardcoded: tracked in Project Backlog GBL-001. Subject to Phase 7 global cleanup.
                _statsText.text = "???";
                _conditionsText.text = data.ConditionsText;
                _skillListView.gameObject.SetActive(false);
                _restartButton.gameObject.SetActive(false);
            }
            else
            {
                _characterNameText.text = data.CharacterName;
                _statsText.text = data.StatsText;
                _conditionsText.text = data.ConditionsText;

                if (data.SkillIcons != null && data.SkillIcons.Length > 0)
                {
                    _skillListView.gameObject.SetActive(true);
                    var skillData = new ReplaySkillDisplayData[data.SkillIcons.Length];
                    for (int i = 0; i < data.SkillIcons.Length; i++)
                    {
                        skillData[i] = new ReplaySkillDisplayData { Icon = data.SkillIcons[i] };
                    }
                    _skillListView.SetSkills(skillData);
                }
                else
                {
                    _skillListView.gameObject.SetActive(false);
                }

                _restartButton.gameObject.SetActive(data.CanRestart);
            }
        }

        public void Hide()
        {
            _popupRoot.SetActive(false);
        }

        private void Awake()
        {
            _restartButton.onClick.AddListener(HandleRestartClicked);
            _closeButton.onClick.AddListener(HandleCloseClicked);
            _dimButton.onClick.AddListener(HandleCloseClicked);
            _skillListView.OnSkillSlotClicked += HandleSkillSlotClicked;
            _popupRoot.SetActive(false);
        }

        private void OnDestroy()
        {
            _restartButton?.onClick.RemoveListener(HandleRestartClicked);
            _closeButton?.onClick.RemoveListener(HandleCloseClicked);
            _dimButton?.onClick.RemoveListener(HandleCloseClicked);
            if (_skillListView != null)
                _skillListView.OnSkillSlotClicked -= HandleSkillSlotClicked;
        }

        private void HandleRestartClicked()
        {
            OnRestartClicked?.Invoke();
        }

        private void HandleCloseClicked()
        {
            OnCloseClicked?.Invoke();
        }

        private void HandleSkillSlotClicked(int index)
        {
            OnSkillSlotClicked?.Invoke(index);
        }

        private void Reset()
        {
            _popupRoot = gameObject;
        }
    }
}
