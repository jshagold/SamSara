using System;
using UnityEngine;

namespace Samsara.Features.ReplayScene.Presentation.Popup
{
    public struct ReplaySkillDisplayData
    {
        public Sprite Icon;
    }

    public class ReplaySkillListView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(ReplaySkillListView)}]";

        [SerializeField] private ReplaySkillSlotView[] _skillSlots;

        public event Action<int> OnSkillSlotClicked;

        public void SetSkills(ReplaySkillDisplayData[] skills)
        {
            for (int i = 0; i < _skillSlots.Length; i++)
            {
                if (i < skills.Length)
                {
                    _skillSlots[i].Setup(i, skills[i].Icon);
                    _skillSlots[i].gameObject.SetActive(true);
                }
                else
                {
                    _skillSlots[i].gameObject.SetActive(false);
                }
            }
        }

        private void Awake()
        {
            for (int i = 0; i < _skillSlots.Length; i++)
            {
                _skillSlots[i].OnSkillSlotClicked += HandleSkillSlotClicked;
            }
        }

        private void OnDestroy()
        {
            if (_skillSlots == null) return;
            for (int i = 0; i < _skillSlots.Length; i++)
            {
                if (_skillSlots[i] != null)
                    _skillSlots[i].OnSkillSlotClicked -= HandleSkillSlotClicked;
            }
        }

        private void HandleSkillSlotClicked(int index)
        {
            OnSkillSlotClicked?.Invoke(index);
        }

        private void Reset()
        {
            _skillSlots = GetComponentsInChildren<ReplaySkillSlotView>();
        }
    }
}
