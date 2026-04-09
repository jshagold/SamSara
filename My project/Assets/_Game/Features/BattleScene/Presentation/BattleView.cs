using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Samsara.Features.BattleScene.Presentation.ActionOrder;
using Samsara.Features.BattleScene.Presentation.Damage;
using Samsara.Features.BattleScene.Presentation.Field;
using Samsara.Features.BattleScene.Presentation.Info;
using Samsara.Features.BattleScene.Presentation.QTE;
using Samsara.Features.BattleScene.Presentation.Result;
using Samsara.Features.BattleScene.Presentation.Skill;
using Samsara.Features.BattleScene.Presentation.TopBar;
using UnityEngine;
using UnityEngine.UI;

namespace Samsara.Features.BattleScene.Presentation
{
    public class BattleView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(BattleView)}]";

        // ── 기존 뷰 참조 ──
        [SerializeField] private TurnNumberView _turnNumberView;
        [SerializeField] private OptionButtonView _optionButtonView;
        [SerializeField] private ActionOrderView _actionOrderView;
        [SerializeField] private AllyFieldView _allyFieldView;
        [SerializeField] private EnemyFieldView _enemyFieldView;
        [SerializeField] private SkillSelectionView _skillSelectionView;
        [SerializeField] private BattleQTEView _battleQTEView;
        [SerializeField] private DamagePopupView _damagePopupView;
        [SerializeField] private BattleResultPopupView _battleResultPopupView;
        [SerializeField] private Image _backgroundImage;

        // ── v2.0.0 신규 뷰 참조 ──
        [SerializeField] private ActionSlotView _actionSlotView;
        [SerializeField] private ConfirmButtonView _confirmButtonView;
        [SerializeField] private BattleStartView _battleStartView;
        [SerializeField] private InfoTooltipView _infoTooltipView;

        // ── 적 턴 정보 패널 (스킬 UI와 같은 하단 영역 공유) ──
        [SerializeField] private EnemyTurnInfoView _enemyTurnInfoView;

        // ── 스킬 영역 슬라이드 루트 ──
        [SerializeField] private RectTransform _skillAreaRect;  // SkillSelection + ActionSlot + Confirm 부모

        private Vector2 _skillAreaOriginalPos;
        private const float SlideOffsetY = 1000f;
        private const float SlideDuration = 0.3f;

        // ── Child View Accessors ──
        public TurnNumberView TurnNumber         => _turnNumberView;
        public OptionButtonView OptionButton     => _optionButtonView;
        public ActionOrderView ActionOrder       => _actionOrderView;
        public AllyFieldView AllyField           => _allyFieldView;
        public EnemyFieldView EnemyField         => _enemyFieldView;
        public SkillSelectionView SkillSelection => _skillSelectionView;
        public BattleQTEView BattleQTE           => _battleQTEView;
        public DamagePopupView DamagePopup       => _damagePopupView;
        public BattleResultPopupView BattleResultPopup => _battleResultPopupView;
        public ActionSlotView ActionSlot         => _actionSlotView;
        public ConfirmButtonView Confirm         => _confirmButtonView;
        public BattleStartView BattleStart       => _battleStartView;
        public InfoTooltipView InfoTooltip       => _infoTooltipView;
        public EnemyTurnInfoView EnemyTurnInfo  => _enemyTurnInfoView;

        private void Awake()
        {
            if (_skillAreaRect != null)
                _skillAreaOriginalPos = _skillAreaRect.anchoredPosition;
        }

        // ── Relayed Events (기존) ──
        public event Action<int> OnSkillSelected
        {
            add => _skillSelectionView.OnSkillSelected += value;
            remove => _skillSelectionView.OnSkillSelected -= value;
        }

        public event Action<int> OnSkillLongPress
        {
            add => _skillSelectionView.OnSkillLongPress += value;
            remove => _skillSelectionView.OnSkillLongPress -= value;
        }

        public event Action<int> OnTargetSelected
        {
            add => _enemyFieldView.OnTargetSelected += value;
            remove => _enemyFieldView.OnTargetSelected -= value;
        }

        public event Action OnOptionClicked
        {
            add => _optionButtonView.OnOptionClicked += value;
            remove => _optionButtonView.OnOptionClicked -= value;
        }

        public event Action OnResultConfirm
        {
            add => _battleResultPopupView.OnConfirm += value;
            remove => _battleResultPopupView.OnConfirm -= value;
        }

        // ── Relayed Events (v2.0.0 신규) ──
        public event Action OnWaitSelected
        {
            add => _actionSlotView.OnWaitSelected += value;
            remove => _actionSlotView.OnWaitSelected -= value;
        }

        public event Action OnConfirmPressed
        {
            add => _confirmButtonView.OnConfirm += value;
            remove => _confirmButtonView.OnConfirm -= value;
        }

        public event Action<int> OnQueueSlotTouched
        {
            add => _actionOrderView.OnSlotTouched += value;
            remove => _actionOrderView.OnSlotTouched -= value;
        }

        // ── Background ──
        public void SetBackground(Sprite sprite)
        {
            _backgroundImage.sprite = sprite;
        }

        // ──────────────────────────────────────────────
        // Skill UI Show / Hide (immediate)
        // ──────────────────────────────────────────────

        public void ShowSkillUI()
        {
            if (_skillAreaRect == null) return;
            _skillAreaRect.anchoredPosition = _skillAreaOriginalPos;
            _skillAreaRect.gameObject.SetActive(true);
        }

        public void HideSkillUI()
        {
            if (_skillAreaRect == null) return;
            _skillAreaRect.gameObject.SetActive(false);
        }

        // ──────────────────────────────────────────────
        // UI Transitions (async)
        // ──────────────────────────────────────────────

        /// <summary>스킬 영역 슬라이드아웃 → QTE 패널 슬라이드인.</summary>
        public async UniTask TransitionToQTE(bool isDefense)
        {
            if (_skillAreaRect != null && _skillAreaRect.gameObject.activeSelf)
            {
                await _skillAreaRect
                    .DOAnchorPosY(_skillAreaOriginalPos.y - SlideOffsetY, SlideDuration)
                    .SetEase(Ease.InQuad).AsyncWaitForCompletion();
                _skillAreaRect.gameObject.SetActive(false);
            }

            await _battleQTEView.SlideIn(isDefense);
        }

        /// <summary>QTE 패널 슬라이드아웃 → 스킬 영역 슬라이드인.</summary>
        public async UniTask TransitionToSkillUI()
        {
            await _battleQTEView.SlideOut();

            if (_skillAreaRect != null)
            {
                _skillAreaRect.anchoredPosition = _skillAreaOriginalPos + Vector2.down * SlideOffsetY;
                _skillAreaRect.gameObject.SetActive(true);
                await _skillAreaRect
                    .DOAnchorPosY(_skillAreaOriginalPos.y, SlideDuration)
                    .SetEase(Ease.OutQuad).AsyncWaitForCompletion();
            }
        }

        // ──────────────────────────────────────────────
        // Enemy Turn Panel
        // ──────────────────────────────────────────────

        /// <summary>
        /// 스킬 UI를 숨기고 하단 영역에 적 턴 정보 패널을 표시한다.
        /// 텍스트 형식: "적 [이름]이(가) [스킬명] 사용"
        /// </summary>
        public void ShowEnemyTurnPanel(string text)
        {
            HideSkillUI();
            _enemyTurnInfoView.Show(text);
        }

        /// <summary>적 턴 정보 패널을 숨긴다. (아군 턴 시작 시 ShowSkillUI가 별도 호출됨)</summary>
        public void HideEnemyTurnPanel()
        {
            _enemyTurnInfoView.Hide();
        }

        private void OnDestroy()
        {
            _skillAreaRect?.DOKill();
        }

        private void Reset()
        {
            _turnNumberView = GetComponentInChildren<TurnNumberView>();
            _optionButtonView = GetComponentInChildren<OptionButtonView>();
            _actionOrderView = GetComponentInChildren<ActionOrderView>();
            _allyFieldView = GetComponentInChildren<AllyFieldView>();
            _enemyFieldView = GetComponentInChildren<EnemyFieldView>();
            _skillSelectionView = GetComponentInChildren<SkillSelectionView>();
            _battleQTEView = GetComponentInChildren<BattleQTEView>();
            _damagePopupView = GetComponentInChildren<DamagePopupView>();
            _battleResultPopupView = GetComponentInChildren<BattleResultPopupView>();
            _actionSlotView = GetComponentInChildren<ActionSlotView>();
            _confirmButtonView = GetComponentInChildren<ConfirmButtonView>();
            _battleStartView = GetComponentInChildren<BattleStartView>();
            _infoTooltipView = GetComponentInChildren<InfoTooltipView>();
            _enemyTurnInfoView = GetComponentInChildren<EnemyTurnInfoView>();
            _backgroundImage = GetComponent<Image>();
        }
    }
}
