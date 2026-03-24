using Cysharp.Threading.Tasks;
using Samsara.Core.Navigation;
using Samsara.Features.MainScene.Domain;
using UnityEngine;

namespace Samsara.Features.MainScene.Presentation
{
    public class MainPresenter
    {
        private readonly string _logClass = $"[{nameof(MainPresenter)}]";

        private readonly MainUseCase _mainUseCase;
        private readonly MainView _mainView;
        private readonly ISceneNavigator _sceneNavigator;

        private bool _isHudVisible = true;

        public MainPresenter(MainUseCase mainUseCase, MainView mainView, ISceneNavigator sceneNavigator)
        {
            _mainUseCase = mainUseCase;
            _mainView = mainView;
            _sceneNavigator = sceneNavigator;
        }

        public void Initialize()
        {
            var viewModel = _mainUseCase.GetMainViewModel();

            _mainView.SetGold(viewModel.Gold);
            _mainView.SetDay(viewModel.Day);
            _mainView.SetHp(viewModel.CurrentHp, viewModel.MaxHp);
            _mainView.SetActionPoints(viewModel.ActionPoints, viewModel.MaxActionPoints);
            _mainView.SetMerchantVisible(viewModel.IsMerchantActive);

            _mainView.OnHudToggleClicked += HandleHudToggle;
            _mainView.OnStageClicked += HandleStageClicked;
            _mainView.OnMaintenanceClicked += HandleMaintenanceClicked;
            _mainView.OnCharacterInfoClicked += HandleCharacterInfoClicked;
            _mainView.OnMerchantClicked += HandleMerchantClicked;
            _mainView.OnOptionClicked += HandleOptionClicked;

            Debug.Log($"{_logClass} Initialize 완료.");
        }

        private void HandleHudToggle()
        {
            _isHudVisible = !_isHudVisible;

            if (_isHudVisible)
                _mainView.ShowHud();
            else
                _mainView.HideHud();
        }

        private void HandleStageClicked()
        {
            _sceneNavigator.NavigateToAsync(SceneKey.Stage).Forget();
        }

        private void HandleMaintenanceClicked()
        {
            _sceneNavigator.NavigateToAsync(SceneKey.Maintenance).Forget();
        }

        private void HandleCharacterInfoClicked()
        {
            _sceneNavigator.NavigateToAsync(SceneKey.CharacterInfo).Forget();
        }

        private void HandleMerchantClicked()
        {
            Debug.Log($"{_logClass} Merchant 클릭 — 아직 미구현.");
        }

        private void HandleOptionClicked()
        {
            Debug.Log($"{_logClass} Option 클릭 — 아직 미구현.");
        }
    }
}
