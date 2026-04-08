using System;
using Samsara.Features.BattleScene.Domain;
using UnityEngine;

namespace Samsara.Features.BattleScene.Presentation.ActionOrder
{
    /// <summary>
    /// 행동 순서 큐 뷰. 5개의 ActionOrderSlotView로 구성.
    /// predicted[0] = 현재 액터 (하이라이트), [1]~[4] = 예측 순서.
    /// </summary>
    public class ActionOrderView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(ActionOrderView)}]";

        [SerializeField] private ActionOrderSlotView[] _slots;  // Inspector에서 5개 연결

        public event Action<int> OnSlotTouched;

        private void Awake()
        {
            for (int i = 0; i < _slots.Length; i++)
            {
                if (_slots[i] != null)
                    _slots[i].OnSlotTouched += HandleSlotTouched;
            }
        }

        /// <summary>
        /// 행동 순서를 슬롯에 표시. predicted[0] = 현재 액터 (자동 하이라이트).
        /// predicted가 null이거나 비어있으면 모든 슬롯을 숨긴다.
        /// </summary>
        public void SetOrder(BattleParticipant[] predicted)
        {
            int count = predicted != null ? Mathf.Min(predicted.Length, _slots.Length) : 0;

            for (int i = 0; i < _slots.Length; i++)
            {
                if (i < count && predicted[i] != null)
                {
                    var p = predicted[i];
                    string name = !string.IsNullOrEmpty(p.DisplayName) ? p.DisplayName : p.SpriteKey;
                    _slots[i].gameObject.SetActive(true);
                    _slots[i].Setup(p.Id, p.PortraitSpriteKey, name, p.IsAlly);
                    _slots[i].SetHighlight(i == 0);  // index 0 = 현재 액터
                }
                else
                {
                    _slots[i].gameObject.SetActive(false);
                }
            }
        }

        /// <summary>participantId와 일치하는 슬롯에 하이라이트를 표시.</summary>
        public void HighlightCurrent(int participantId)
        {
            for (int i = 0; i < _slots.Length; i++)
            {
                if (_slots[i] == null || !_slots[i].gameObject.activeSelf) continue;
                _slots[i].SetHighlight(_slots[i].ParticipantId == participantId);
            }
        }

        private void HandleSlotTouched(int participantId)
        {
            OnSlotTouched?.Invoke(participantId);
        }

        private void OnDestroy()
        {
            for (int i = 0; i < _slots.Length; i++)
            {
                if (_slots[i] != null)
                    _slots[i].OnSlotTouched -= HandleSlotTouched;
            }
        }

        private void Reset()
        {
            _slots = GetComponentsInChildren<ActionOrderSlotView>();
        }
    }
}
