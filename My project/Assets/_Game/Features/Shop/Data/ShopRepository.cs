using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Samsara.Features.Shop.Domain;
using UnityEngine;

namespace Samsara.Features.Shop.Data
{
    public class ShopRepository : IShopRepository
    {
        private readonly string _logClass = $"[{nameof(ShopRepository)}]";

        private readonly string _savePath;
        private ShopRunData _runData;
        private bool _isDirty;

        public ShopRepository()
        {
            _savePath = Path.Combine(Application.persistentDataPath, "shop_run.json");
            _runData  = new ShopRunData();
        }

        // ── Load / Save ──

        public async UniTask LoadDataAsync()
        {
            if (!File.Exists(_savePath))
            {
                _runData = new ShopRunData();
                Debug.Log($"{_logClass} 저장 파일 없음. 기본값으로 초기화.");
                return;
            }

            string json = await UniTask.RunOnThreadPool(() => File.ReadAllText(_savePath));
            _runData  = JsonConvert.DeserializeObject<ShopRunData>(json) ?? new ShopRunData();
            _isDirty  = false;
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

        public int GetActiveMerchantId()    => _runData.ActiveMerchantId;
        public int GetMerchantAppearedDay() => _runData.MerchantAppearedDay;

        public Dictionary<int, int> GetRemainingStock()
            => _runData.RemainingStock;

        // ── Write ──

        public void SetActiveMerchant(int merchantId, int appearedDay, Dictionary<int, int> stock)
        {
            _runData.ActiveMerchantId    = merchantId;
            _runData.MerchantAppearedDay = appearedDay;
            _runData.RemainingStock      = new Dictionary<int, int>(stock);
            _isDirty = true;
        }

        public void ClearActiveMerchant()
        {
            _runData.ActiveMerchantId    = -1;
            _runData.MerchantAppearedDay = 0;
            _runData.RemainingStock.Clear();
            _isDirty = true;
        }

        public void DecrementStock(int potionId)
        {
            if (!_runData.RemainingStock.ContainsKey(potionId)) return;
            _runData.RemainingStock[potionId]--;
            _isDirty = true;
        }

        public void ResetRunData()
        {
            _runData = new ShopRunData();
            _isDirty = true;
        }
    }
}
