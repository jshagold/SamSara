using Cysharp.Threading.Tasks;
using Samsara.Core.MasterData;
using Samsara.Core.Navigation;
using Samsara.Core.Popup;
using Samsara.Features.CharacterInfoScene.Domain;
using Samsara.Features.CharacterInfoScene.Presentation.InfoScroll;

namespace Samsara.Features.CharacterInfoScene.Presentation
{
    public class CharacterInfoPresenter
    {
        private readonly string _logClass = $"[{nameof(CharacterInfoPresenter)}]";

        private readonly CharacterInfoUseCase _useCase;
        private readonly CharacterInfoView    _view;
        private readonly ISceneNavigator      _sceneNavigator;
        private readonly IPopupManager        _popupManager;

        private SkillSO[] _skillSoCache;

        public CharacterInfoPresenter(
            CharacterInfoUseCase useCase,
            CharacterInfoView    view,
            ISceneNavigator      sceneNavigator,
            IPopupManager        popupManager)
        {
            _useCase        = useCase;
            _view           = view;
            _sceneNavigator = sceneNavigator;
            _popupManager   = popupManager;
        }

        public void Initialize()
        {
            var evolutionNode = _useCase.GetCurrentEvolutionNode();

            // Character sprite — Phase 1 placeholder (no Addressables loading yet)
            _view.CharacterSpriteView.SetSprite(null);

            // Evolution stage button
            _view.EvolutionStageButtonView.SetEvolutionInfo(null, evolutionNode.CharacterName);

            // Stats
            var stats = _useCase.GetCurrentStats();
            _view.InfoScrollView.StatListView.SetStats(
                stats.Hp, stats.Strength, stats.Toughness, stats.Agility);

            // Character name
            _view.InfoScrollView.CharacterNameView.SetName(evolutionNode.CharacterName);

            // Skills
            _skillSoCache = _useCase.GetSkills();
            var skillDisplayData = new SkillDisplayData[_skillSoCache.Length];
            for (int i = 0; i < _skillSoCache.Length; i++)
            {
                skillDisplayData[i] = new SkillDisplayData
                {
                    Icon      = null, // Phase 1 placeholder
                    HasEffect = _skillSoCache[i].Effects != null && _skillSoCache[i].Effects.Length > 0
                };
            }
            _view.InfoScrollView.SkillListView.SetSkills(skillDisplayData);

            // Subscribe events
            _view.BackButtonView.OnBackClicked                          += HandleBackClicked;
            _view.OptionButtonView.OnOptionClicked                      += HandleOptionClicked;
            _view.EvolutionStageButtonView.OnEvolutionStageClicked      += HandleEvolutionStageClicked;
            _view.InfoScrollView.SkillListView.OnSkillSlotClicked       += HandleSkillSlotClicked;
            _view.SkillDescriptionPopupView.OnCloseClicked              += HandleSkillPopupClose;
        }

        private void HandleBackClicked()
        {
            _sceneNavigator.NavigateToAsync(SceneKey.Main).Forget();
        }

        private void HandleOptionClicked()
        {
            // Phase 1 stub — no option menu yet
        }

        private void HandleEvolutionStageClicked()
        {
            _sceneNavigator.NavigateToAsync(SceneKey.EvolutionTree).Forget();
        }

        private void HandleSkillSlotClicked(int index)
        {
            if (_skillSoCache == null || index < 0 || index >= _skillSoCache.Length) return;

            var skill = _skillSoCache[index];
            var effectDescription = BuildEffectDescription(skill);
            _view.SkillDescriptionPopupView.Show(
                null,
                skill.SkillName,
                skill.Description,
                skill.Damage,
                effectDescription);
        }

        private void HandleSkillPopupClose()
        {
            _view.SkillDescriptionPopupView.Hide();
        }

        private string BuildEffectDescription(SkillSO skill)
        {
            if (skill.Effects == null || skill.Effects.Length == 0) return string.Empty;

            var sb = new System.Text.StringBuilder();
            foreach (var effect in skill.Effects)
                sb.AppendLine($"{effect.EffectType} ({effect.Duration}s)");
            return sb.ToString().TrimEnd();
        }

        public void Dispose()
        {
            if (_view == null) return;
            _view.BackButtonView.OnBackClicked                     -= HandleBackClicked;
            _view.OptionButtonView.OnOptionClicked                 -= HandleOptionClicked;
            _view.EvolutionStageButtonView.OnEvolutionStageClicked -= HandleEvolutionStageClicked;
            _view.InfoScrollView.SkillListView.OnSkillSlotClicked  -= HandleSkillSlotClicked;
            _view.SkillDescriptionPopupView.OnCloseClicked         -= HandleSkillPopupClose;
        }
    }
}
