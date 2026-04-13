using Samsara.Features.Shop.MasterData;

namespace Samsara.Features.Shop.Domain
{
    /// <summary>상점 아이템 표시용 DTO.</summary>
    public class ShopItemInfo
    {
        public PotionSO Potion         { get; }
        public int      RemainingStock { get; }
        public int      Price          { get; }

        public ShopItemInfo(PotionSO potion, int remainingStock, int price)
        {
            Potion         = potion;
            RemainingStock = remainingStock;
            Price          = price;
        }
    }
}
