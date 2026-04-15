using Cysharp.Threading.Tasks;
using Samsara.Core.MasterData;
using Samsara.Core.Navigation;
using Samsara.Core.Popup;
using Samsara.Features.Character.MasterData;
using Samsara.Features.CharacterInfoScene.Domain;
using Samsara.Features.CharacterInfoScene.Presentation.InfoScroll;
using Samsara.Features.Inventory.Domain;
using UnityEngine;

namespace Samsara.Features.CharacterInfoScene.Presentation
{
    public class CharacterInfoPresenter
    {
        private readonly string _logClass = $"[{nameof(CharacterInfoPresenter)}]";

        private readonly CharacterInfoUseCase _useCase;
        private readonly InventoryUseCase     _inventoryUseCase;
        private readonly CharacterInfoView    _view;
        private readonly ISceneNavigator      _sceneNavigator;
        private readonly IPopupManager        _popupManager;

        private SkillSO[] _skillSoCache;
        private int       _currentDetailSlotIndex = -1;

        public CharacterInfoPresenter(
            CharacterInfoUseCase useCase,
            InventoryUseCase     inventoryUseCase,
            CharacterInfoView    view,
            ISceneNavigator      sceneNavigator,
            IPopupManager        popupManager)
        {
            _useCase          = useCase;
            _inventoryUseCase = inventoryUseCase;
            _view             = view;
            _sceneNavigator   = sceneNavigator;
            _popupManager     = popupManager;
        }

        public void Initialize()
        {
            var evolutionNode = _useCase.GetCurrentEvolutionNode();

            // Phase 1: 캐릭터 스프라이트 placeholder (ISpriteLoader 미주입)
            _view.CharacterSprite.SetSprite(null);

            // 진화 단계 버튼 (Phase 1: 아이콘 없음)
            _view.EvolutionStageButton.SetEvolutionInfo(null, evolutionNode.CharacterName);

            // 스탯
            RefreshStats();

            // 캐릭터 이름
            _view.InfoScroll.CharacterName.SetName(evolutionNode.CharacterName);

            // 스킬 — SkillSO 캐시 후 SkillDisplayData 변환
            _skillSoCache = _useCase.GetSkills();
            var skillDisplayData = new SkillDisplayData[_skillSoCache.Length];
            for (int i = 0; i < _skillSoCache.Length; i++)
            {
                skillDisplayData[i] = new SkillDisplayData
                {
                    Icon      = null, // Phase 1: ISpriteLoader 미주입
                    HasEffect = _skillSoCache[i].Effects != null && _skillSoCache[i].Effects.Length > 0
                };
            }
            _view.InfoScroll.SkillList.SetSkills(skillDisplayData);

            // 인벤토리 슬롯 초기화
            RefreshInventory();

            // 이벤트 구독
            _view.BackButton.OnBackClicked                      += HandleBackClicked;
            _view.OptionButton.OnOptionClicked                  += HandleOptionClicked;
            _view.EvolutionStageButton.OnEvolutionStageClicked  += HandleEvolutionStageClicked;
            _view.InfoScroll.SkillList.OnSkillSlotClicked       += HandleSkillSlotClicked;
            _view.SkillDescriptionPopup.OnCloseClicked          += HandleSkillPopupClose;
            _view.InfoScroll.Inventory.OnSlotClicked            += HandleInventorySlotClicked;
            _view.ItemDetailPopup.OnUseClicked                  += HandleUseItemClicked;
            _view.ItemDetailPopup.OnDiscardClicked              += HandleDiscardItemClicked;
            _view.ItemDetailPopup.OnCloseClicked                += HandleItemDetailPopupClose;

            Debug.Log($"{_logClass} Initialize 완료.");
        }

        public void Dispose()
        {
            _view.BackButton.OnBackClicked                     -= HandleBackClicked;
            _view.OptionButton.OnOptionClicked                 -= HandleOptionClicked;
            _view.EvolutionStageButton.OnEvolutionStageClicked -= HandleEvolutionStageClicked;
            _view.InfoScroll.SkillList.OnSkillSlotClicked      -= HandleSkillSlotClicked;
            _view.SkillDescriptionPopup.OnCloseClicked         -= HandleSkillPopupClose;
            _view.InfoScroll.Inventory.OnSlotClicked           -= HandleInventorySlotClicked;
            _view.ItemDetailPopup.OnUseClicked                 -= HandleUseItemClicked;
            _view.ItemDetailPopup.OnDiscardClicked             -= HandleDiscardItemClicked;
            _view.ItemDetailPopup.OnCloseClicked               -= HandleItemDetailPopupClose;

            Debug.Log($"{_logClass} Dispose 완료.");
        }

        // ── Helpers ──

        private void RefreshStats()
        {
            var stats = _useCase.GetCurrentStats();
            _view.InfoScroll.StatList.SetStats(
                stats.Hp, stats.Strength, stats.Toughness, stats.Agility);
        }

        private void RefreshInventory()
        {
            var slotData = _inventoryUseCase.GetSlotDisplayData();
            _view.InfoScroll.Inventory.SetSlots(slotData);
        }

        // ── Event Handlers ──

        private void HandleBackClicked()
        {
            _sceneNavigator.NavigateToAsync(SceneKey.Main).Forget();
        }

        private void HandleOptionClicked()
        {
            Debug.Log($"{_logClass} Option 클릭 — 미구현.");
        }

        private void HandleEvolutionStageClicked()
        {
            _popupManager.ShowConfirmAsync(
                new PopupRequest("준비 중", "진화 트리는 준비 중입니다.", "확인")).Forget();
        }

        private void HandleSkillSlotClicked(int index)
        {
            if (_skillSoCache == null || index < 0 || index >= _skillSoCache.Length) return;

            var skill = _skillSoCache[index];

            string effectDescription;
            if (skill.Effects == null || skill.Effects.Length == 0)
            {
                effectDescription = "없음";
            }
            else
            {
                var sb = new System.Text.StringBuilder();
                for (int i = 0; i < skill.Effects.Length; i++)
                {
                    if (i > 0) sb.Append(", ");
                    sb.Append($"{skill.Effects[i].EffectType} {skill.Effects[i].Duration}s");
                }
                effectDescription = sb.ToString();
            }

            _view.SkillDescriptionPopup.Show(
                null, // Phase 1: ISpriteLoader 미주입
                skill.SkillName,
                skill.Description,
                skill.Damage,
                effectDescription);
        }

        private void HandleSkillPopupClose()
        {
            _view.SkillDescriptionPopup.Hide();
        }

        private void HandleInventorySlotClicked(int slotIndex)
        {
            var detail = _inventoryUseCase.GetItemDetail(slotIndex);
            if (detail == null) return;

            _currentDetailSlotIndex = slotIndex;
            _view.ItemDetailPopup.Show(detail);
        }

        private void HandleUseItemClicked()
        {
            UseItemAsync().Forget();
        }

        private void HandleDiscardItemClicked()
        {
            DiscardItemAsync().Forget();
        }

        private void HandleItemDetailPopupClose()
        {
            _currentDetailSlotIndex = -1;
            _view.ItemDetailPopup.Hide();
        }

        // ── Async Operations ──

        private async UniTaskVoid UseItemAsync()
        {
            if (_currentDetailSlotIndex < 0) return;

            var result = await _inventoryUseCase.UseItem(_currentDetailSlotIndex);

            if (!result.IsSuccess)
            {
                Debug.Log($"{_logClass} 아이템 사용 실패: {result.FailReason}");
                return;
            }

            // 결과 피드백
            string statName = result.AffectedStat switch
            {
                StatType.Hp        => "HP",
                StatType.Strength  => "공격력",
                StatType.Toughness => "방어력",
                StatType.Agility   => "민첩",
                _                  => result.AffectedStat.ToString()
            };
            await _popupManager.ShowConfirmAsync(
                new PopupRequest("아이템 사용", $"{statName} +{result.EffectValue} 증가!", "확인"));

            // 팝업 닫기 및 화면 갱신
            _currentDetailSlotIndex = -1;
            _view.ItemDetailPopup.Hide();
            RefreshStats();
            RefreshInventory();
        }

        private async UniTaskVoid DiscardItemAsync()
        {
            if (_currentDetailSlotIndex < 0) return;

            // 버리기 전 확인
            bool confirmed = await _popupManager.ShowYesNoAsync(
                new PopupRequest("아이템 버리기", "정말 버리시겠습니까?", "버리기", "취소"));

            if (!confirmed) return;

            bool success = await _inventoryUseCase.DiscardItem(_currentDetailSlotIndex, 1);

            if (!success)
            {
                Debug.LogWarning($"{_logClass} 버리기 실패: slotIndex={_currentDetailSlotIndex}");
                return;
            }

            // 팝업 닫기 및 인벤토리 갱신
            _currentDetailSlotIndex = -1;
            _view.ItemDetailPopup.Hide();
            RefreshInventory();
        }
    }
}
