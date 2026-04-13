using System;
using UnityEngine;

namespace Samsara.Features.Shop.MasterData
{
    [Serializable]
    public class MerchantSaleItem
    {
        [SerializeField] private int _potionId;
        [SerializeField] private int _stock;

        public int PotionId => _potionId;
        public int Stock    => _stock;
    }
}
