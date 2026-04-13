using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Samsara.Features.Character.MasterData;
using Samsara.Features.Event.MasterData;
using Samsara.Features.MaintenanceScene.Presentation.Main;
using Samsara.Features.MaintenanceScene.Presentation.Shop;
using Samsara.Features.MaintenanceScene.Presentation.TopBar;
using Samsara.Features.MaintenanceScene.Presentation.Training;
using Samsara.Features.Shop.Domain;
using Samsara.Features.Shop.Presentation;
using UnityEngine;

namespace Samsara.Features.MaintenanceScene.Presentation
{
    public class MaintenanceView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(MaintenanceView)}]";

        [SerializeField] private TopBarView                      _topBarView;
        [SerializeField] private CharacterInfoPanelView          _characterInfoPanelView;
        [SerializeField] private InteractionOptionsView          _interactionOptionsView;
        [SerializeField] private TrainingListView                _trainingListView;
        [SerializeField] private ShopView                        _shopView;          // legacy stub — kept for backward compat
        [SerializeField] private BackgroundView                  _backgroundView;
        [SerializeField] private ShopPanelView                   _shopPanelView;
        [SerializeField] private Samsara.Features.Event.Presentation.EventDialogueOverlayController _dialogueOverlay;

        // Events — forwarded from child Views
        public event Action          OnBackClicked;
        public event Action          OnOptionClicked;
        public event Action          OnTrainingClicked;
        public event Action          OnExplorationClicked;
        public event Action          OnShopClicked;
        public event Action<StatType> OnStatSelected;
        public event Action          OnTrainingListClosed;
        public event Action          OnShopCloseClicked;
        public event Action<int>     OnShopPurchaseRequested;

        private void Awake()
        {
            _topBarView.OnBackClicked                    += () => OnBackClicked?.Invoke();
            _topBarView.OnOptionClicked                  += () => OnOptionClicked?.Invoke();
            _interactionOptionsView.OnTrainingClicked    += () => OnTrainingClicked?.Invoke();
            _interactionOptionsView.OnExplorationClicked += () => OnExplorationClicked?.Invoke();
            _interactionOptionsView.OnShopClicked        += () => OnShopClicked?.Invoke();
            _trainingListView.OnStatSelected             += statType => OnStatSelected?.Invoke(statType);
            _trainingListView.OnCloseClicked             += () => OnTrainingListClosed?.Invoke();
            _shopPanelView.OnCloseClicked                += () => OnShopCloseClicked?.Invoke();
            _shopPanelView.OnPurchaseRequested           += potionId => OnShopPurchaseRequested?.Invoke(potionId);
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
            _shopPanelView.Hide();
        }

        /// <summary>훈련 목록 모드로 전환. items 데이터로 TrainingListView를 갱신한다.</summary>
        public void ShowTrainingListMode(TrainingItemData[] items)
        {
            _interactionOptionsView.gameObject.SetActive(false);
            _trainingListView.Show(items);
            _shopPanelView.Hide();
        }

        public void ShowShopPanelMode(List<ShopItemInfo> items, int gold, Sprite merchantSprite)
        {
            _interactionOptionsView.gameObject.SetActive(false);
            _trainingListView.Hide();
            _shopPanelView.SetMerchantSprite(merchantSprite);
            _shopPanelView.Show(items, gold);
        }

        // ── Shop Panel updates ──

        public void UpdateShopGold(int gold)
            => _shopPanelView.UpdateGold(gold);

        public void UpdateShopSlot(int slotIndex, int remainingStock)
            => _shopPanelView.UpdateSlot(slotIndex, remainingStock);

        // ── Merchant Dialogue ──

        /// <summary>
        /// 상인 대화 오버레이를 실행하고 선택된 인덱스를 반환한다.
        /// 0 = 거래, 1 = 떠나보내기 (choiceTexts 순서 기준).
        /// </summary>
        public UniTask<int> RunMerchantDialogueAsync(EventDialogue[] dialogues, string[] choiceTexts)
            => _dialogueOverlay.RunDialoguesWithChoicesAsync(dialogues, choiceTexts);
    }
}
