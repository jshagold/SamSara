using Cysharp.Threading.Tasks;
using Samsara.Core.AssetLoading;
using Samsara.Core.Navigation;
using Samsara.Features.Character.MasterData;
using Samsara.Features.MainScene.Domain;
using UnityEngine;

namespace Samsara.Features.MainScene.Presentation
{
    public class MainPresenter
    {
        private readonly string _logClass = $"[{nameof(MainPresenter)}]";

        private readonly MainUseCase      _mainUseCase;
        private readonly MainView         _mainView;
        private readonly ISceneNavigator  _sceneNavigator;
        private readonly ISpriteLoader    _spriteLoader;
        private readonly EvolutionNodeSO  _evolutionNode;

        private bool _isHudVisible = true;

        public MainPresenter(
            MainUseCase      mainUseCase,
            MainView         mainView,
            ISceneNavigator  sceneNavigator,
            ISpriteLoader    spriteLoader,
            EvolutionNodeSO  evolutionNode)
        {
            _mainUseCase    = mainUseCase;
            _mainView       = mainView;
            _sceneNavigator = sceneNavigator;
            _spriteLoader   = spriteLoader;
            _evolutionNode  = evolutionNode;
        }

        public void Initialize()
        {
            InitializeAsync().Forget();
        }

        private async UniTaskVoid InitializeAsync()
        {
            var viewModel = _mainUseCase.GetMainViewModel();

            _mainView.SetGold(viewModel.Gold);
            _mainView.SetDay(viewModel.Day);
            _mainView.SetHp(viewModel.CurrentHp, viewModel.MaxHp);
            _mainView.SetActionPoints(viewModel.ActionPoints, viewModel.MaxActionPoints);

            _mainView.OnHudToggleClicked  += HandleHudToggle;
            _mainView.OnStageClicked      += HandleStageClicked;
            _mainView.OnMaintenanceClicked += HandleMaintenanceClicked;
            _mainView.OnCharacterInfoClicked += HandleCharacterInfoClicked;
            _mainView.OnOptionClicked     += HandleOptionClicked;

            // Sprite 로드 (EvolutionNodeSO 키 기반)
            if (_evolutionNode != null)
            {
                var characterSprite = await _spriteLoader.LoadSpriteAsync(_evolutionNode.MainStandingSpriteKey);
                _mainView.SetCharacterSprite(characterSprite);

                var portrait = await _spriteLoader.LoadSpriteAsync(_evolutionNode.PortraitSpriteKey);
                _mainView.SetPortrait(portrait);
            }

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

        private void HandleOptionClicked()
        {
            Debug.Log($"{_logClass} Option 클릭 — 아직 미구현.");
        }
    }
}
