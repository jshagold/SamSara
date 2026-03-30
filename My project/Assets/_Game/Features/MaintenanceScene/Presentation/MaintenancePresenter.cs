using Cysharp.Threading.Tasks;
using Samsara.Core.Navigation;
using Samsara.Core.Popup;
using Samsara.Features.Character.MasterData;
using Samsara.Features.MaintenanceScene.Domain;
using Samsara.Features.MaintenanceScene.Presentation.Training;
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

        // 캐시된 핸들러 — Dispose 시 정확한 구독 해제를 위해 보관
        private System.Action<StatType> _statSelectedHandler;

        public MaintenancePresenter(
            MaintenanceUseCase maintenanceUseCase,
            MaintenanceView    maintenanceView,
            ISceneNavigator    sceneNavigator,
            IPopupManager      popupManager)
        {
            _maintenanceUseCase = maintenanceUseCase;
            _maintenanceView    = maintenanceView;
            _sceneNavigator     = sceneNavigator;
            _popupManager       = popupManager;
        }

        public void Initialize()
        {
            var vm = _maintenanceUseCase.GetMaintenanceViewModel();

            _maintenanceView.SetCharacterInfo(vm);
            _maintenanceView.SetShopButtonVisible(_maintenanceUseCase.IsMerchantAvailable());
            _maintenanceView.SetInteractable(_maintenanceUseCase.CanPerformAction());
            _maintenanceView.ShowDefaultMode();

            // 이벤트 구독
            _statSelectedHandler = statType => HandleStatSelectedAsync(statType).Forget();

            _maintenanceView.OnBackClicked          += HandleBackClicked;
            _maintenanceView.OnOptionClicked        += HandleOptionClicked;
            _maintenanceView.OnTrainingClicked      += HandleTrainingClicked;
            _maintenanceView.OnExplorationClicked   += HandleExplorationClickedAsync;
            _maintenanceView.OnShopClicked          += HandleShopClicked;
            _maintenanceView.OnStatSelected         += _statSelectedHandler;
            _maintenanceView.OnTrainingListClosed += HandleTrainingListClosed;
            _maintenanceView.OnShopCloseClicked     += HandleShopClosed;

            Debug.Log($"{_logClass} Initialize 완료.");
        }

        public void Dispose()
        {
            _maintenanceView.OnBackClicked          -= HandleBackClicked;
            _maintenanceView.OnOptionClicked        -= HandleOptionClicked;
            _maintenanceView.OnTrainingClicked      -= HandleTrainingClicked;
            _maintenanceView.OnExplorationClicked   -= HandleExplorationClickedAsync;
            _maintenanceView.OnShopClicked          -= HandleShopClicked;
            _maintenanceView.OnStatSelected         -= _statSelectedHandler;
            _maintenanceView.OnTrainingListClosed -= HandleTrainingListClosed;
            _maintenanceView.OnShopCloseClicked     -= HandleShopClosed;

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

            _maintenanceUseCase.ConsumeActionPoint();
            _maintenanceView.SetInteractable(_maintenanceUseCase.CanPerformAction());

            // TODO: [SPEC-GAP] D-04 — 훈련 미니게임 씬이 Tasks에 정의되어 있지 않음. 전환 스킵.
            Debug.Log($"{_logClass} {statType} 훈련 선택 — 미니게임 씬 미정의, 전환 스킵.");
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

        private void HandleShopClicked()
        {
            _maintenanceView.ShowShopMode();
        }

        private void HandleTrainingListClosed()
        {
            _maintenanceView.ShowDefaultMode();
        }

        private void HandleShopClosed()
        {
            _maintenanceView.ShowDefaultMode();
        }
    }
}
