using System;
using UnityEngine;
using UnityEngine.UI;

namespace Samsara.Features.ReplayScene.Presentation.Popup
{
    public class ReplaySkillSlotView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(ReplaySkillSlotView)}]";

        [SerializeField] private Button _button;
        [SerializeField] private Image _skillIcon;

        public int SkillIndex { get; private set; }

        public event Action<int> OnSkillSlotClicked;

        public void Setup(int index, Sprite icon)
        {
            SkillIndex = index;
            _skillIcon.sprite = icon;
        }

        private void Awake()
        {
            // Lambda capture exception (§2-3 (1) standard pattern) — RemoveAllListeners allowed
            _button.onClick.AddListener(() => OnSkillSlotClicked?.Invoke(SkillIndex));
        }

        private void OnDestroy()
        {
            _button?.onClick.RemoveAllListeners();
        }

        private void Reset()
        {
            _button = GetComponentInChildren<Button>();
            _skillIcon = GetComponentInChildren<Image>();
        }
    }
}
