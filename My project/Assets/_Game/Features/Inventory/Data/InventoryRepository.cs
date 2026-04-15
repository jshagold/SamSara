using System.IO;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Samsara.Features.Inventory.Domain;
using UnityEngine;

namespace Samsara.Features.Inventory.Data
{
    public class InventoryRepository : IInventoryRepository
    {
        private readonly string _logClass = $"[{nameof(InventoryRepository)}]";

        private readonly string _savePath;
        private InventoryRunData _runData;
        private bool _isDirty;

        public InventoryRepository()
        {
            _savePath = Application.persistentDataPath + "/inventory_run_data.json";
            _runData  = new InventoryRunData();
        }

        // ── Load / Save ──

        public async UniTask LoadDataAsync()
        {
            if (!File.Exists(_savePath))
            {
                _runData = new InventoryRunData();
                Debug.Log($"{_logClass} 저장 파일 없음. 기본값으로 초기화.");
                return;
            }

            string json = await UniTask.RunOnThreadPool(() => File.ReadAllText(_savePath));
            _runData = JsonConvert.DeserializeObject<InventoryRunData>(json) ?? new InventoryRunData();
            _isDirty = false;
            Debug.Log($"{_logClass} 데이터 로드 완료.");
        }

        public async UniTask SaveDataAsync()
        {
            if (!_isDirty) return;

            var snapshot = _runData;
            await UniTask.RunOnThreadPool(() =>
            {
                var json = JsonConvert.SerializeObject(snapshot);
                File.WriteAllText(_savePath, json);
            });
            _isDirty = false;
            Debug.Log($"{_logClass} 비동기 저장 완료.");
        }

        public void SaveDataSync()
        {
            if (!_isDirty) return;

            var json = JsonConvert.SerializeObject(_runData);
            File.WriteAllText(_savePath, json);
            _isDirty = false;
            Debug.Log($"{_logClass} 동기 저장 완료.");
        }

        // ── Read ──

        public InventorySlotData[] GetSlots() => _runData.Slots;

        // ── Write ──

        public void SetSlot(int slotIndex, int itemId, int quantity)
        {
            _runData.Slots[slotIndex].ItemId   = itemId;
            _runData.Slots[slotIndex].Quantity = quantity;
            _isDirty = true;
        }

        public void ClearSlot(int slotIndex)
        {
            _runData.Slots[slotIndex] = new InventorySlotData();
            _isDirty = true;
        }

        public void ResetRunData()
        {
            _runData = new InventoryRunData();
            _isDirty = true;
        }

        public void MarkDirty() => _isDirty = true;
    }
}
