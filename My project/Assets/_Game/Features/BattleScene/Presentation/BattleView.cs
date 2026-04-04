using System;
using Samsara.Features.BattleScene.Presentation.ActionOrder;
using Samsara.Features.BattleScene.Presentation.Damage;
using Samsara.Features.BattleScene.Presentation.Field;
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

        // ── Child View Accessors ──
        public TurnNumberView TurnNumber => _turnNumberView;
        public OptionButtonView OptionButton => _optionButtonView;
        public ActionOrderView ActionOrder => _actionOrderView;
        public AllyFieldView AllyField => _allyFieldView;
        public EnemyFieldView EnemyField => _enemyFieldView;
        public SkillSelectionView SkillSelection => _skillSelectionView;
        public BattleQTEView BattleQTE => _battleQTEView;
        public DamagePopupView DamagePopup => _damagePopupView;
        public BattleResultPopupView BattleResultPopup => _battleResultPopupView;

        // ── Relayed Events ──
        public event Action<int> OnSkillSelected
        {
            add => _skillSelectionView.OnSkillSelected += value;
            remove => _skillSelectionView.OnSkillSelected -= value;
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

        public void SetBackground(Sprite sprite)
        {
            _backgroundImage.sprite = sprite;
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
            _backgroundImage = GetComponent<Image>();
        }
    }
}
