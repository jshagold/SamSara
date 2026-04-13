using Samsara.Features.Shop.MasterData;

namespace Samsara.Features.Shop.Domain
{
    public interface IShopMasterDataRepository
    {
        MerchantSO   GetMerchant(int merchantId);
        MerchantSO[] GetAllMerchants();
        PotionSO     GetPotion(int potionId);
        PotionSO[]   GetAllPotions();
    }
}
