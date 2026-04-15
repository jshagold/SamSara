using Samsara.Features.Inventory.Domain;
using UnityEngine;

namespace Samsara.Features.CharacterInfoScene.Presentation.InfoScroll
{
    /// <summary>
    /// 인벤토리 슬롯 목록 View. Phase 2 — 슬롯 표시 및 클릭 이벤트 지원.
    /// </summary>
    public class InventoryView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(InventoryView)}]";

        [SerializeField] private InventorySlotView[] _slots;

        public event System.Action<int> OnSlotClicked;

        private void Awake()
        {
            for (int i = 0; i < _slots.Length; i++)
            {
                var slot = _slots[i];
                slot.OnSlotClicked += index => OnSlotClicked?.Invoke(index);
            }
        }

        /// <summary>슬롯 목록을 갱신한다. data.Length 와 _slots.Length 가 다를 경우 가능한 범위만 반영.</summary>
        public void SetSlots(InventorySlotDisplayData[] data)
        {
            int count = Mathf.Min(_slots.Length, data.Length);
            for (int i = 0; i < count; i++)
                _slots[i].Setup(i, data[i]);

            // 데이터보다 슬롯이 많으면 나머지는 빈 상태
            for (int i = count; i < _slots.Length; i++)
                _slots[i].Setup(i, new InventorySlotDisplayData { IsEmpty = true });
        }

        // Reset() 미구현 — InventorySlotView[] 자동 배정 불가. Inspector에서 수동 배정 필요.
    }
}
