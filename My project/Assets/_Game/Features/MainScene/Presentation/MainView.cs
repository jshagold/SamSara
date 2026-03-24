using System;
using Samsara.Features.MainScene.Presentation.Hud;
using Samsara.Features.MainScene.Presentation.Main;
using UnityEngine;

namespace Samsara.Features.MainScene.Presentation
{
    public class MainView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(MainView)}]";

        // HUD Views
        [SerializeField] private GoldView _goldView;
        [SerializeField] private DayView _dayView;
        [SerializeField] private HudToggleButtonView _hudToggleButtonView;
        [SerializeField] private OptionButtonView _optionButtonView;
        [SerializeField] private CharacterStatusView _characterStatusView;
        [SerializeField] private HudView _hudView;

        // Main Views
        [SerializeField] private CharacterSpriteView _characterSpriteView;
        [SerializeField] private StageButtonView _stageButtonView;
        [SerializeField] private MaintenanceButtonView _maintenanceButtonView;
        [SerializeField] private CharacterInfoButtonView _characterInfoButtonView;
        [SerializeField] private MerchantButtonView _merchantButtonView;

        // Events — forwarded from child Views
        public event Action OnHudToggleClicked;
        public event Action OnOptionClicked;
        public event Action OnStageClicked;
        public event Action OnMaintenanceClicked;
        public event Action OnCharacterInfoClicked;
        public event Action OnMerchantClicked;

        private void Awake()
        {
            _hudToggleButtonView.OnToggleClicked += () => OnHudToggleClicked?.Invoke();
            _optionButtonView.OnOptionClicked += () => OnOptionClicked?.Invoke();
            _stageButtonView.OnButtonClicked += () => OnStageClicked?.Invoke();
            _maintenanceButtonView.OnButtonClicked += () => OnMaintenanceClicked?.Invoke();
            _characterInfoButtonView.OnButtonClicked += () => OnCharacterInfoClicked?.Invoke();
            _merchantButtonView.OnMerchantClicked += () => OnMerchantClicked?.Invoke();
        }

        // HUD delegates
        public void SetGold(int gold) => _goldView.SetGold(gold);
        public void SetDay(int day) => _dayView.SetDay(day);
        public void SetHp(int current, int max) => _characterStatusView.SetHp(current, max);
        public void SetActionPoints(int current, int max) => _characterStatusView.SetActionPoints(current, max);
        public void SetPortrait(Sprite sprite) => _characterStatusView.SetPortrait(sprite);
        public void ShowHud() => _hudView.ShowHud();
        public void HideHud() => _hudView.HideHud();

        // Main delegates
        public void SetMerchantVisible(bool visible) => _merchantButtonView.SetVisible(visible);
    }
}
