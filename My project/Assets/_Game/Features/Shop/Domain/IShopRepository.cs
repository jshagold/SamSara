using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Samsara.Features.Shop.Domain
{
    public interface IShopRepository
    {
        UniTask LoadDataAsync();
        UniTask SaveDataAsync();
        void    SaveDataSync();

        int                 GetActiveMerchantId();
        int                 GetMerchantAppearedDay();
        void                SetActiveMerchant(int merchantId, int appearedDay, Dictionary<int, int> stock);
        void                ClearActiveMerchant();
        Dictionary<int, int> GetRemainingStock();
        void                DecrementStock(int potionId);
        void                InitializeNewRun(RunConfigSO config);
        void                ResetRunData();
    }
}
