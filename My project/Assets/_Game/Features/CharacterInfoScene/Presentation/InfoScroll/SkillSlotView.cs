using System;
using UnityEngine;
using UnityEngine.UI;

namespace Samsara.Features.CharacterInfoScene.Presentation.InfoScroll
{
    public class SkillSlotView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(SkillSlotView)}]";

        [SerializeField] private Button     _button;
        [SerializeField] private Image      _skillIcon;
        [SerializeField] private GameObject _effectIndicator;

        public int SkillIndex { get; private set; }

        public event Action<int> OnSkillSlotClicked;

        private void Awake()
        {
            _button.onClick.AddListener(() => OnSkillSlotClicked?.Invoke(SkillIndex));
        }

        /// <summary>슬롯 초기 설정. SkillListView.SetSkills()에서 호출.</summary>
        public void Setup(int index, Sprite icon, bool hasEffect)
        {
            SkillIndex              = index;
            _skillIcon.sprite       = icon;
            _effectIndicator.SetActive(hasEffect);
        }

        private void Reset()
        {
            _button    = GetComponentInChildren<Button>();
            _skillIcon = GetComponentInChildren<Image>();
            // _effectIndicator는 Inspector에서 수동 배정
        }

        private void OnDestroy()
        {
            _button?.onClick.RemoveAllListeners();
        }
    }
}
