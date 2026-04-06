using System;
using UnityEngine;

namespace Samsara.Features.CharacterInfoScene.Presentation.InfoScroll
{
    public struct SkillDisplayData
    {
        public UnityEngine.Sprite Icon;
        public bool               HasEffect;
    }

    public class SkillListView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(SkillListView)}]";

        [SerializeField] private SkillSlotView[] _skillSlots;

        public event Action<int> OnSkillSlotClicked;

        private void Awake()
        {
            foreach (var slot in _skillSlots)
                slot.OnSkillSlotClicked += index => OnSkillSlotClicked?.Invoke(index);
        }

        public void SetSkills(SkillDisplayData[] skills)
        {
            for (int i = 0; i < _skillSlots.Length; i++)
            {
                if (i < skills.Length)
                {
                    _skillSlots[i].gameObject.SetActive(true);
                    _skillSlots[i].Setup(i, skills[i].Icon, skills[i].HasEffect);
                }
                else
                {
                    _skillSlots[i].gameObject.SetActive(false);
                }
            }
        }

        private void Reset()
        {
            _skillSlots = GetComponentsInChildren<SkillSlotView>();
        }
    }
}
