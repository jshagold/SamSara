using System;
using Samsara.Features.Character.MasterData;
using Samsara.Features.MaintenanceScene.Presentation.Main;
using Samsara.Features.MaintenanceScene.Presentation.Shop;
using Samsara.Features.MaintenanceScene.Presentation.TopBar;
using Samsara.Features.MaintenanceScene.Presentation.Training;
using UnityEngine;

namespace Samsara.Features.MaintenanceScene.Presentation
{
    public class MaintenanceView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(MaintenanceView)}]";

        [SerializeField] private TopBarView _topBarView;
        [SerializeField] private CharacterInfoPanelView _characterInfoPanelView;
        [SerializeField] private InteractionOptionsView _interactionOptionsView;
        [SerializeField] private TrainingListView _trainingListView;
        [SerializeField] private ShopView _shopView;
        [SerializeField] private BackgroundView _backgroundView;

        // Events — forwarded from child Views
        public event Action OnBackClicked;
        public event Action OnOptionClicked;
        public event Action OnTrainingClicked;
        public event Action OnExplorationClicked;
        public event Action OnShopClicked;
        public event Action<StatType> OnStatSelected;
        public event Action OnTrainingListClosed;
        public event Action OnShopCloseClicked;

        private void Awake()
        {
            _topBarView.OnBackClicked              += () => OnBackClicked?.Invoke();
            _topBarView.OnOptionClicked            += () => OnOptionClicked?.Invoke();
            _interactionOptionsView.OnTrainingClicked    += () => OnTrainingClicked?.Invoke();
            _interactionOptionsView.OnExplorationClicked += () => OnExplorationClicked?.Invoke();
            _interactionOptionsView.OnShopClicked        += () => OnShopClicked?.Invoke();
            _trainingListView.OnStatSelected       += statType => OnStatSelected?.Invoke(statType);
            _trainingListView.OnCloseClicked    += () => OnTrainingListClosed?.Invoke();
            _shopView.OnCloseClicked               += () => OnShopCloseClicked?.Invoke();
        }

        // ── CharacterInfoPanel delegates ──

        public void SetCharacterInfo(Domain.MaintenanceViewModel viewModel)
            => _characterInfoPanelView.SetCharacterInfo(viewModel);

        // ── InteractionOptions delegates ──

        public void SetShopButtonVisible(bool visible)
            => _interactionOptionsView.SetShopButtonVisible(visible);

        public void SetInteractable(bool interactable)
            => _interactionOptionsView.SetInteractable(interactable);

        // ── Mode switch ──

        public void ShowDefaultMode()
        {
            _interactionOptionsView.gameObject.SetActive(true);
            _trainingListView.Hide();
            _shopView.Hide();
        }

        /// <summary>훈련 목록 모드로 전환. items 데이터로 TrainingListView를 갱신한다.</summary>
        public void ShowTrainingListMode(TrainingItemData[] items)
        {
            _interactionOptionsView.gameObject.SetActive(false);
            _trainingListView.Show(items);
            _shopView.Hide();
        }

        public void ShowShopMode()
        {
            _interactionOptionsView.gameObject.SetActive(false);
            _trainingListView.Hide();
            _shopView.Show();
        }
    }
}
