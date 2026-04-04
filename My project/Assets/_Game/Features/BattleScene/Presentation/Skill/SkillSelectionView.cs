using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

namespace Samsara.Features.BattleScene.Presentation.Skill
{
    public struct SkillDisplayData
    {
        public int SkillId;
        public string SpriteKey;
        public int CooldownRemaining;
        public bool IsUsable;
    }

    public class SkillSelectionView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(SkillSelectionView)}]";

        [SerializeField] private Transform _skillContainer;
        [SerializeField] private Button _skillButtonPrefab;

        private ObjectPool<Button> _buttonPool;
        private readonly List<Button> _activeButtons = new List<Button>();
        private readonly Dictionary<int, Button> _buttonBySkillId = new Dictionary<int, Button>();
        private readonly Dictionary<int, SkillDisplayData> _skillDataMap = new Dictionary<int, SkillDisplayData>();

        public event Action<int> OnSkillSelected;

        private void Awake()
        {
            _buttonPool = new ObjectPool<Button>(
                createFunc: () => Instantiate(_skillButtonPrefab, _skillContainer),
                actionOnGet: btn => btn.gameObject.SetActive(true),
                actionOnRelease: btn =>
                {
                    btn.onClick.RemoveAllListeners();
                    btn.gameObject.SetActive(false);
                },
                actionOnDestroy: btn => Destroy(btn.gameObject),
                defaultCapacity: 6,
                maxSize: 12
            );
        }

        public void SetSkills(SkillDisplayData[] skills)
        {
            ClearSkills();

            foreach (var skill in skills)
            {
                var btn = _buttonPool.Get();
                int capturedId = skill.SkillId;
                btn.onClick.AddListener(() => OnSkillSelected?.Invoke(capturedId));
                btn.interactable = skill.IsUsable;

                var label = btn.GetComponentInChildren<TMP_Text>();
                if (label != null)
                {
                    if (skill.CooldownRemaining > 0)
                        label.text = $"CD:{skill.CooldownRemaining}";
                    else
                        label.text = $"Skill {skill.SkillId}";
                }

                _activeButtons.Add(btn);
                _buttonBySkillId[skill.SkillId] = btn;
                _skillDataMap[skill.SkillId] = skill;
            }
        }

        public void SetActive(bool active)
        {
            foreach (var btn in _activeButtons)
                btn.interactable = active && _skillDataMap.ContainsKey(GetSkillIdForButton(btn))
                    && _skillDataMap[GetSkillIdForButton(btn)].IsUsable;

            var canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup != null)
                canvasGroup.alpha = active ? 1f : 0.4f;
        }

        public void RefreshCooldowns(Dictionary<int, int> cooldowns)
        {
            foreach (var kvp in cooldowns)
            {
                if (!_buttonBySkillId.TryGetValue(kvp.Key, out var btn)) continue;

                bool isUsable = kvp.Value <= 0;
                btn.interactable = isUsable;

                var label = btn.GetComponentInChildren<TMP_Text>();
                if (label != null)
                {
                    label.text = kvp.Value > 0 ? $"CD:{kvp.Value}" : $"Skill {kvp.Key}";
                }
            }
        }

        private int GetSkillIdForButton(Button btn)
        {
            foreach (var kvp in _buttonBySkillId)
            {
                if (kvp.Value == btn) return kvp.Key;
            }
            return -1;
        }

        private void ClearSkills()
        {
            foreach (var btn in _activeButtons)
                _buttonPool.Release(btn);

            _activeButtons.Clear();
            _buttonBySkillId.Clear();
            _skillDataMap.Clear();
        }

        private void OnDestroy()
        {
            ClearSkills();
            _buttonPool?.Dispose();
        }

        private void Reset()
        {
            _skillContainer = transform;
        }
    }
}
