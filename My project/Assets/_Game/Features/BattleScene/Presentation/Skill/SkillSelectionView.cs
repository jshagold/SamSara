using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Pool;
using UnityEngine.UI;

namespace Samsara.Features.BattleScene.Presentation.Skill
{
    public enum SkillState
    {
        Usable,           // 정상 사용 가능
        OnCooldown,       // 쿨다운 중 (dimA — 어두운 오버레이 + 남은 턴 수)
        HpInsufficient    // HP 부족 (dimB — 붉은 오버레이)
    }

    public struct SkillDisplayData
    {
        public int SkillId;
        public string SpriteKey;
        public int CooldownRemaining;
        public bool IsUsable;
        public SkillState State;  // 3-state dim 제어
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

        private CancellationTokenSource _longPressCts;
        private const float LongPressThreshold = 0.5f;

        // Normal 색상 (기준)
        private static readonly Color ColorUsable = Color.white;
        private static readonly Color ColorOnCooldown = new Color(0.3f, 0.3f, 0.3f, 1f);    // dimA — 어두운
        private static readonly Color ColorHpInsufficient = new Color(1f, 0.25f, 0.25f, 1f); // dimB — 붉은

        public event Action<int> OnSkillSelected;
        public event Action<int> OnSkillLongPress;

        private void Awake()
        {
            _buttonPool = new ObjectPool<Button>(
                createFunc: () => Instantiate(_skillButtonPrefab, _skillContainer),
                actionOnGet: btn => btn.gameObject.SetActive(true),
                actionOnRelease: btn =>
                {
                    btn.onClick.RemoveAllListeners();
                    RemoveLongPressEventTrigger(btn);
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
                btn.interactable = skill.State == SkillState.Usable;

                // 텍스트 표시
                var label = btn.GetComponentInChildren<TMP_Text>();
                if (label != null)
                {
                    label.text = skill.State == SkillState.OnCooldown
                        ? $"CD:{skill.CooldownRemaining}"
                        : $"Skill {skill.SkillId}";
                }

                // 3-state 색상
                ApplySkillState(btn, skill.State);

                // 롱프레스 이벤트 트리거 연결
                SetupLongPressEventTrigger(btn, capturedId);

                _activeButtons.Add(btn);
                _buttonBySkillId[skill.SkillId] = btn;
                _skillDataMap[skill.SkillId] = skill;
            }
        }

        public void SetActive(bool active)
        {
            var canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup != null)
                canvasGroup.alpha = active ? 1f : 0.4f;

            foreach (var btn in _activeButtons)
            {
                int id = GetSkillIdForButton(btn);
                if (id < 0) continue;
                bool usable = _skillDataMap.ContainsKey(id) && _skillDataMap[id].State == SkillState.Usable;
                btn.interactable = active && usable;
            }
        }

        public void RefreshCooldowns(Dictionary<int, int> cooldowns)
        {
            foreach (var kvp in cooldowns)
            {
                if (!_buttonBySkillId.TryGetValue(kvp.Key, out var btn)) continue;
                if (!_skillDataMap.TryGetValue(kvp.Key, out var data)) continue;

                bool isUsable = kvp.Value <= 0;
                data.CooldownRemaining = kvp.Value;
                data.State = isUsable ? SkillState.Usable : SkillState.OnCooldown;
                data.IsUsable = isUsable;
                _skillDataMap[kvp.Key] = data;

                btn.interactable = isUsable;
                ApplySkillState(btn, data.State);

                var label = btn.GetComponentInChildren<TMP_Text>();
                if (label != null)
                    label.text = kvp.Value > 0 ? $"CD:{kvp.Value}" : $"Skill {kvp.Key}";
            }
        }

        // ──────────────────────────────────────────────
        // 3-State Visual
        // ──────────────────────────────────────────────

        private static void ApplySkillState(Button btn, SkillState state)
        {
            var img = btn.GetComponent<Image>();
            if (img == null) return;

            img.color = state switch
            {
                SkillState.OnCooldown => ColorOnCooldown,
                SkillState.HpInsufficient => ColorHpInsufficient,
                _ => ColorUsable
            };
        }

        // ──────────────────────────────────────────────
        // Long-Press (EventTrigger)
        // ──────────────────────────────────────────────

        private void SetupLongPressEventTrigger(Button btn, int skillId)
        {
            var trigger = btn.gameObject.GetComponent<EventTrigger>()
                          ?? btn.gameObject.AddComponent<EventTrigger>();
            trigger.triggers.Clear();

            var downEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
            downEntry.callback.AddListener(_ => HandleSkillPointerDown(skillId));
            trigger.triggers.Add(downEntry);

            var upEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
            upEntry.callback.AddListener(_ => HandleSkillPointerUp());
            trigger.triggers.Add(upEntry);
        }

        private static void RemoveLongPressEventTrigger(Button btn)
        {
            var trigger = btn.gameObject.GetComponent<EventTrigger>();
            if (trigger != null)
                trigger.triggers.Clear();
        }

        private void HandleSkillPointerDown(int skillId)
        {
            _longPressCts?.Cancel();
            _longPressCts?.Dispose();
            _longPressCts = new CancellationTokenSource();
            StartLongPressTimer(skillId, _longPressCts.Token).Forget();
        }

        private void HandleSkillPointerUp()
        {
            _longPressCts?.Cancel();
            _longPressCts?.Dispose();
            _longPressCts = null;
        }

        private async UniTaskVoid StartLongPressTimer(int skillId, CancellationToken token)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(LongPressThreshold), cancellationToken: token);
            if (!token.IsCancellationRequested)
                OnSkillLongPress?.Invoke(skillId);
        }

        // ──────────────────────────────────────────────
        // Helpers
        // ──────────────────────────────────────────────

        private int GetSkillIdForButton(Button btn)
        {
            foreach (var kvp in _buttonBySkillId)
                if (kvp.Value == btn) return kvp.Key;
            return -1;
        }

        private void ClearSkills()
        {
            _longPressCts?.Cancel();
            _longPressCts?.Dispose();
            _longPressCts = null;

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
