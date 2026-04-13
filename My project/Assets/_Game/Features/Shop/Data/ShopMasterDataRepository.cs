using System.Collections.Generic;
using Samsara.Features.Shop.Domain;
using Samsara.Features.Shop.MasterData;
using UnityEngine;

namespace Samsara.Features.Shop.Data
{
    public class ShopMasterDataRepository : IShopMasterDataRepository
    {
        private readonly string _logClass = $"[{nameof(ShopMasterDataRepository)}]";

        private readonly Dictionary<int, PotionSO>   _potions   = new Dictionary<int, PotionSO>();
        private readonly Dictionary<int, MerchantSO> _merchants = new Dictionary<int, MerchantSO>();

        public ShopMasterDataRepository()
        {
            var potions = Resources.LoadAll<PotionSO>("MasterData/Shop");
            foreach (var p in potions)
                _potions[p.Id] = p;

            var merchants = Resources.LoadAll<MerchantSO>("MasterData/Shop");
            foreach (var m in merchants)
                _merchants[m.Id] = m;

            Debug.Log($"{_logClass} 포션 {_potions.Count}개, 상인 {_merchants.Count}개 로드 완료.");
        }

        public PotionSO GetPotion(int potionId)
        {
            if (_potions.TryGetValue(potionId, out var potion)) return potion;
            throw new System.InvalidOperationException($"{_logClass} PotionSO not found: id={potionId}");
        }

        public PotionSO[] GetAllPotions()
        {
            var arr = new PotionSO[_potions.Count];
            _potions.Values.CopyTo(arr, 0);
            return arr;
        }

        public MerchantSO GetMerchant(int merchantId)
        {
            if (_merchants.TryGetValue(merchantId, out var merchant)) return merchant;
            throw new System.InvalidOperationException($"{_logClass} MerchantSO not found: id={merchantId}");
        }

        public MerchantSO[] GetAllMerchants()
        {
            var arr = new MerchantSO[_merchants.Count];
            _merchants.Values.CopyTo(arr, 0);
            return arr;
        }
    }
}
