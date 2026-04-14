using Cysharp.Threading.Tasks;
using Samsara.Core.Navigation;
using Samsara.Core.Popup;
using Samsara.Features.Character.MasterData;
using Samsara.Features.MaintenanceScene.Domain;
using Samsara.Features.MaintenanceScene.Presentation.Training;
using Samsara.Features.Shop.Domain;
using Samsara.Features.Shop.MasterData;
using Samsara.Features.Shop.Presentation;
using UnityEngine;

namespace Samsara.Features.MaintenanceScene.Presentation
{
    public class MaintenancePresenter
    {
        private readonly string _logClass = $"[{nameof(MaintenancePresenter)}]";

        private readonly MaintenanceUseCase _maintenanceUseCase;
        private readonly MaintenanceView    _maintenanceView;
        private readonly ISceneNavigator    _sceneNavigator;
        private readonly IPopupManager      _popupManager;
        private readonly GameContext        _gameContext;
        private readonly ShopUseCase        _shopUseCase;

        // 캐시된 핸들러 — Dispose 시 정확한 구독 해제를 위해 보관
        private System.Action<StatType> _statSelectedHandler;

        public MaintenancePresenter(
            MaintenanceUseCase maintenanceUseCase,
            MaintenanceView    maintenanceView,
            ISceneNavigator    sceneNavigator,
            IPopupManager      popupManager,
            GameContext        gameContext,
            ShopUseCase        shopUseCase)
        {
            _maintenanceUseCase = maintenanceUseCase;
            _maintenanceView    = maintenanceView;
            _sceneNavigator     = sceneNavigator;
            _popupManager       = popupManager;
            _gameContext        = gameContext;
            _shopUseCase        = shopUseCase;
        }

        public void Initialize()
        {
            InitializeAsync().Forget();
        }

        private async UniTaskVoid InitializeAsync()
        {
            // 상인 만료 확인 (비동기)
            await _shopUseCase.CheckAndExpireMerchant();

            var vm = _maintenanceUseCase.GetMaintenanceViewModel();

            _maintenanceView.SetCharacterInfo(vm);
            _maintenanceView.SetShopButtonVisible(_shopUseCase.IsShopAvailable());
            _maintenanceView.SetInteractable(_maintenanceUseCase.CanPerformAction());
            _maintenanceView.ShowDefaultMode();

            // 이벤트 구독
            _statSelectedHandler = statType => HandleStatSelectedAsync(statType).Forget();

            _maintenanceView.OnBackClicked          += HandleBackClicked;
            _maintenanceView.OnOptionClicked        += HandleOptionClicked;
            _maintenanceView.OnTrainingClicked      += HandleTrainingClicked;
            _maintenanceView.OnExplorationClicked   += HandleExplorationClickedAsync;
            _maintenanceView.OnShopClicked          += HandleShopClickedAsync;
            _maintenanceView.OnStatSelected         += _statSelectedHandler;
            _maintenanceView.OnTrainingListClosed   += HandleTrainingListClosed;
            _maintenanceView.OnShopCloseClicked     += HandleShopClosed;
            _maintenanceView.OnShopPurchaseRequested += HandlePurchaseRequestedAsync;

            Debug.Log($"{_logClass} Initialize 완료.");
        }

        public void Dispose()
        {
            _maintenanceView.OnBackClicked           -= HandleBackClicked;
            _maintenanceView.OnOptionClicked         -= HandleOptionClicked;
            _maintenanceView.OnTrainingClicked       -= HandleTrainingClicked;
            _maintenanceView.OnExplorationClicked    -= HandleExplorationClickedAsync;
            _maintenanceView.OnShopClicked           -= HandleShopClickedAsync;
            _maintenanceView.OnStatSelected          -= _statSelectedHandler;
            _maintenanceView.OnTrainingListClosed    -= HandleTrainingListClosed;
            _maintenanceView.OnShopCloseClicked      -= HandleShopClosed;
            _maintenanceView.OnShopPurchaseRequested -= HandlePurchaseRequestedAsync;

            Debug.Log($"{_logClass} Dispose 완료.");
        }

        // ── Event Handlers ──

        private void HandleBackClicked()
        {
            _sceneNavigator.NavigateToAsync(SceneKey.Main).Forget();
        }

        private void HandleOptionClicked()
        {
            Debug.Log($"{_logClass} Option 클릭 — 아직 미구현.");
        }

        private void HandleTrainingClicked()
        {
            var vm = _maintenanceUseCase.GetMaintenanceViewModel();
            var items = new TrainingItemData[]
            {
                new TrainingItemData { StatType = StatType.Hp,        MiniGameName = "HP Training",        CurrentValue = vm.Stats[StatType.Hp]        },
                new TrainingItemData { StatType = StatType.Strength,  MiniGameName = "Strength Training",  CurrentValue = vm.Stats[StatType.Strength]  },
                new TrainingItemData { StatType = StatType.Toughness, MiniGameName = "Toughness Training", CurrentValue = vm.Stats[StatType.Toughness] },
                new TrainingItemData { StatType = StatType.Agility,   MiniGameName = "Agility Training",   CurrentValue = vm.Stats[StatType.Agility]   },
            };
            _maintenanceView.ShowTrainingListMode(items);
        }

        private async UniTaskVoid HandleStatSelectedAsync(StatType statType)
        {
            if (!_maintenanceUseCase.CanPerformAction())
            {
                await _popupManager.ShowConfirmAsync(
                    new PopupRequest("행동력 부족", "행동력이 부족합니다.", "확인"));
                return;
            }

            // AP 소비는 MiniGame 완료 시 MiniGameUseCase.ApplyResultAndSave에서 처리 (Patch-001)
            _gameContext.PendingTrainingStat = statType;
            _sceneNavigator.NavigateToAsync(SceneKey.MiniGame).Forget();
        }

        private void HandleExplorationClickedAsync()
        {
            HandleExplorationAsync().Forget();
        }

        private async UniTaskVoid HandleExplorationAsync()
        {
            if (!_maintenanceUseCase.CanPerformAction())
            {
                await _popupManager.ShowConfirmAsync(
                    new PopupRequest("행동력 부족", "행동력이 부족합니다.", "확인"));
                return;
            }

            _maintenanceUseCase.ConsumeActionPoint();
            _maintenanceView.SetInteractable(_maintenanceUseCase.CanPerformAction());

            // TODO: [SPEC-GAP] D-05 — 탐험 씬이 Tasks에 정의되어 있지 않음. 전환 스킵.
            Debug.Log($"{_logClass} 탐험 선택 — 탐험 씬 미정의, 전환 스킵.");
        }

        private void HandleShopClickedAsync()
        {
            ShowMerchantDialogueAsync().Forget();
        }

        private async UniTaskVoid ShowMerchantDialogueAsync()
        {
            var merchant = _shopUseCase.GetActiveMerchantData();
            if (merchant == null)
            {
                Debug.LogWarning($"{_logClass} 상인 데이터 없음.");
                return;
            }

            var greetingDialogues = _shopUseCase.GetGreetingDialogues();
            var eventDialogues    = MerchantDialogueAdapter.ToEventDialogues(greetingDialogues);
            var choices           = new string[] { "거래", "떠나보내기" };

            // 상인 대화 → 선택지 표시 (0: 거래, 1: 떠나보내기)
            int chosen = await _maintenanceView.RunMerchantDialogueAsync(eventDialogues, choices);

            if (chosen == 0)
                ShowShopPanel(merchant);
            else
                _maintenanceView.ShowDefaultMode();
        }

        private void ShowShopPanel(MerchantSO merchant)
        {
            var items = _shopUseCase.GetShopItems();
            int gold  = _gameContext.CharacterRunRepo.RunData.Gold;
            _maintenanceView.ShowShopPanelMode(items, gold, merchant.ShopSprite);
        }

        private void HandleTrainingListClosed()
        {
            _maintenanceView.ShowDefaultMode();
        }

        private void HandleShopClosed()
        {
            _maintenanceView.ShowDefaultMode();
        }

        private void HandlePurchaseRequestedAsync(int potionId)
        {
            ProcessPurchaseAsync(potionId).Forget();
        }

        private async UniTaskVoid ProcessPurchaseAsync(int potionId)
        {
            // 재고 선행 체크 — 재고 없으면 구매 확인 팝업 없이 즉시 차단
            var stock = _gameContext.ShopRepo.GetRemainingStock();
            if (!stock.TryGetValue(potionId, out int remaining) || remaining <= 0)
            {
                await _popupManager.ShowConfirmAsync(
                    new PopupRequest("재고 없음", "재고가 없습니다.", "확인"));
                return;
            }

            // 골드 선행 체크 — 골드 부족하면 구매 확인 팝업 없이 즉시 차단
            var potion = _gameContext.ShopMasterDataRepo.GetPotion(potionId);
            if (_gameContext.CharacterRunRepo.RunData.Gold < potion.Price)
            {
                await _popupManager.ShowConfirmAsync(
                    new PopupRequest("골드 부족", "골드가 부족합니다.", "확인"));
                return;
            }

            bool confirmed = await _popupManager.ShowYesNoAsync(
                new PopupRequest("구매 확인", $"{potion.PotionName} 을(를) {potion.Price}G에 구매하시겠습니까?", "구매", "취소"));

            if (!confirmed) return;

            var result = await _shopUseCase.PurchasePotion(potionId);

            switch (result)
            {
                case PurchaseResult.Success:
                    // 골드 및 슬롯 갱신
                    int newGold = _gameContext.CharacterRunRepo.RunData.Gold;
                    _maintenanceView.UpdateShopGold(newGold);
                    _maintenanceView.UpdateTopBarGold(newGold);
                    var items = _shopUseCase.GetShopItems();
                    for (int i = 0; i < items.Count; i++)
                    {
                        if (items[i].Potion.Id == potionId)
                        {
                            _maintenanceView.UpdateShopSlot(i, items[i].RemainingStock);
                            break;
                        }
                    }
                    break;

                case PurchaseResult.InsufficientGold:
                    await _popupManager.ShowConfirmAsync(
                        new PopupRequest("골드 부족", "골드가 부족합니다.", "확인"));
                    break;

                case PurchaseResult.OutOfStock:
                    // 선행 체크 이후 race condition 대응 (이론상 도달 불가)
                    await _popupManager.ShowConfirmAsync(
                        new PopupRequest("재고 없음", "재고가 없습니다.", "확인"));
                    break;
            }
        }
    }
}
