using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Samsara.Features.Character.Data;
using Samsara.Features.Character.Domain;
using Samsara.Features.Event.MasterData;
using Samsara.Features.Inventory.Domain;
using Samsara.Features.Shop.MasterData;
using UnityEngine;

namespace Samsara.Features.Shop.Domain
{
    /// <summary>
    /// 상점 비즈니스 로직. 순수 C# — MonoBehaviour 금지.
    /// 상태를 보유하지 않고 Repository를 통해 데이터에 접근한다.
    /// </summary>
    public class ShopUseCase
    {
        private readonly string _logClass = $"[{nameof(ShopUseCase)}]";

        private readonly IShopRepository            _shopRepo;
        private readonly IShopMasterDataRepository  _masterDataRepo;
        private readonly ICharacterRunRepository    _characterRunRepo;
        private readonly InventoryUseCase           _inventoryUseCase;

        public ShopUseCase(
            IShopRepository           shopRepo,
            IShopMasterDataRepository masterDataRepo,
            ICharacterRunRepository   characterRunRepo,
            InventoryUseCase          inventoryUseCase)
        {
            _shopRepo         = shopRepo;
            _masterDataRepo   = masterDataRepo;
            _characterRunRepo = characterRunRepo;
            _inventoryUseCase = inventoryUseCase;
        }

        // ──────────────────────────────────────────────
        // Merchant Lifecycle
        // ──────────────────────────────────────────────

        /// <summary>
        /// 상인을 활성화한다. ShopEncounter 이벤트 결과에서 호출.
        /// currentDay는 내부적으로 CharacterRunRepository에서 가져온다.
        /// </summary>
        public async UniTask ActivateMerchant(int merchantId)
        {
            var merchant   = _masterDataRepo.GetMerchant(merchantId);
            var currentDay = _characterRunRepo.RunData.Day;

            var initialStock = new Dictionary<int, int>();
            foreach (var item in merchant.SaleItems)
                initialStock[item.PotionId] = item.Stock;

            _shopRepo.SetActiveMerchant(merchantId, currentDay, initialStock);
            await _shopRepo.SaveDataAsync();

            Debug.Log($"{_logClass} 상인 활성화: id={merchantId}, day={currentDay}");
        }

        /// <summary>
        /// 상인 체류 기간을 확인하고 만료 시 비활성화한다.
        /// MaintenanceScene 진입 시 호출.
        /// </summary>
        public async UniTask CheckAndExpireMerchant()
        {
            int merchantId = _shopRepo.GetActiveMerchantId();
            if (merchantId == -1) return;

            var merchant    = _masterDataRepo.GetMerchant(merchantId);
            int appearedDay = _shopRepo.GetMerchantAppearedDay();
            int currentDay  = _characterRunRepo.RunData.Day;

            if (currentDay - appearedDay >= merchant.StayDuration)
            {
                _shopRepo.ClearActiveMerchant();
                await _shopRepo.SaveDataAsync();
                Debug.Log($"{_logClass} 상인 체류 만료: id={merchantId}");
            }
        }

        /// <summary>현재 상인이 활성화되어 있는지 반환한다.</summary>
        public bool IsShopAvailable() => _shopRepo.GetActiveMerchantId() != -1;

        /// <summary>현재 활성 상인 데이터를 반환한다. 상인이 없으면 null.</summary>
        public MerchantSO GetActiveMerchantData()
        {
            int id = _shopRepo.GetActiveMerchantId();
            if (id == -1) return null;
            return _masterDataRepo.GetMerchant(id);
        }

        // ──────────────────────────────────────────────
        // Shop Items
        // ──────────────────────────────────────────────

        /// <summary>현재 상인의 판매 아이템 목록을 반환한다.</summary>
        public List<ShopItemInfo> GetShopItems()
        {
            var merchant = GetActiveMerchantData();
            if (merchant == null) return new List<ShopItemInfo>();

            var stock  = _shopRepo.GetRemainingStock();
            var result = new List<ShopItemInfo>(merchant.SaleItems.Length);

            foreach (var saleItem in merchant.SaleItems)
            {
                var potion = _masterDataRepo.GetPotion(saleItem.PotionId);
                int remaining = stock.TryGetValue(saleItem.PotionId, out int s) ? s : 0;
                result.Add(new ShopItemInfo(potion, remaining, potion.Price));
            }

            return result;
        }

        // ──────────────────────────────────────────────
        // Purchase
        // ──────────────────────────────────────────────

        /// <summary>포션을 구매한다. 인벤토리 확인 → 재고 확인 → 골드 확인 → 골드 차감 + 인벤토리 추가 + 재고 감소 → 저장.</summary>
        public async UniTask<PurchaseResult> PurchasePotion(int potionId)
        {
            // 인벤토리 수용 가능 여부 선행 확인
            if (!_inventoryUseCase.CanAddItem(potionId))
            {
                Debug.Log($"{_logClass} 구매 실패: 인벤토리 가득 참 potionId={potionId}");
                return PurchaseResult.InventoryFull;
            }

            var stock = _shopRepo.GetRemainingStock();
            if (!stock.TryGetValue(potionId, out int remaining) || remaining <= 0)
            {
                Debug.Log($"{_logClass} 구매 실패: 재고 없음 potionId={potionId}");
                return PurchaseResult.OutOfStock;
            }

            var potion  = _masterDataRepo.GetPotion(potionId);
            var runData = _characterRunRepo.RunData;

            if (runData.Gold < potion.Price)
            {
                Debug.Log($"{_logClass} 구매 실패: 골드 부족 need={potion.Price}, have={runData.Gold}");
                return PurchaseResult.InsufficientGold;
            }

            // 골드 차감
            runData.Gold -= potion.Price;
            _characterRunRepo.MarkDirty();

            // 인벤토리에 아이템 추가 (인벤토리 저장은 AddItem 내부에서 처리)
            var addResult = await _inventoryUseCase.AddItem(potionId);
            if (addResult != AddItemResult.Success)
            {
                // CanAddItem 통과 후 race condition 등 예기치 않은 실패 — 방어적 처리
                Debug.LogWarning($"{_logClass} AddItem 예기치 않은 실패: potionId={potionId}, result={addResult}");
                return PurchaseResult.InventoryFull;
            }

            // 재고 감소
            _shopRepo.DecrementStock(potionId);

            // 저장 (골드 변경 + 재고 변경)
            await UniTask.WhenAll(
                _characterRunRepo.SaveDataAsync(),
                _shopRepo.SaveDataAsync()
            );

            Debug.Log($"{_logClass} 구매 성공: potionId={potionId}");
            return PurchaseResult.Success;
        }

        // ──────────────────────────────────────────────
        // Dialogue
        // ──────────────────────────────────────────────

        /// <summary>현재 활성 상인의 인사 대사를 반환한다.</summary>
        public EventDialogue[] GetGreetingDialogues()
        {
            var merchant = GetActiveMerchantData();
            return merchant?.GreetingDialogues ?? new EventDialogue[0];
        }

        // ──────────────────────────────────────────────
        // Run Reset
        // ──────────────────────────────────────────────

        /// <summary>환생 시 상점 런 데이터를 초기화한다.</summary>
        public void ResetRunData() => _shopRepo.ResetRunData();

    }
}
