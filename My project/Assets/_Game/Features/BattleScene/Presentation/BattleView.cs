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
using TMPro;
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

        // ── 턴 레이블 ──
        [SerializeField] private TMP_Text _turnLabel;
        [SerializeField] private CanvasGroup _turnLabelGroup;

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

        private void Awake()
        {
            if (_skillAreaRect != null)
                _skillAreaOriginalPos = _skillAreaRect.anchoredPosition;

            if (_turnLabelGroup != null)
                _turnLabelGroup.alpha = 0f;
            if (_turnLabel != null)
                _turnLabel.gameObject.SetActive(false);
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
        // Turn Labels
        // ──────────────────────────────────────────────

        /// <summary>"[displayName]'s Turn" 텍스트를 표시 후 자동으로 사라진다.</summary>
        public async UniTask ShowEnemyTurnLabel(string displayName)
        {
            await ShowLabel($"{displayName}'s Turn", 1000);
        }

        /// <summary>스킬 이름 레이블을 표시 후 자동으로 사라진다.</summary>
        public async UniTask ShowSkillNameLabel(string skillName)
        {
            await ShowLabel(skillName, 800);
        }

        private async UniTask ShowLabel(string text, int holdMs)
        {
            if (_turnLabel == null || _turnLabelGroup == null) return;

            _turnLabel.text = text;
            _turnLabelGroup.alpha = 0f;
            _turnLabel.gameObject.SetActive(true);

            await _turnLabelGroup.DOFade(1f, 0.2f).SetEase(Ease.OutQuad).AsyncWaitForCompletion();
            await UniTask.Delay(holdMs);
            await _turnLabelGroup.DOFade(0f, 0.2f).SetEase(Ease.InQuad).AsyncWaitForCompletion();

            _turnLabel.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            _skillAreaRect?.DOKill();
            _turnLabelGroup?.DOKill();
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
            _backgroundImage = GetComponent<Image>();
        }
    }
}
