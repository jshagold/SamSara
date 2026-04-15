using Cysharp.Threading.Tasks;
using Samsara.Core.AssetLoading;
using Samsara.Core.MasterData;
using Samsara.Core.Navigation;
using Samsara.Core.Popup;
using Samsara.Features.CharacterInfoScene.Domain;
using Samsara.Features.CharacterInfoScene.Presentation.InfoScroll;
using UnityEngine;

namespace Samsara.Features.CharacterInfoScene.Presentation
{
    public class CharacterInfoPresenter
    {
        private readonly string _logClass = $"[{nameof(CharacterInfoPresenter)}]";

        private readonly CharacterInfoUseCase _useCase;
        private readonly CharacterInfoView    _view;
        private readonly ISceneNavigator      _sceneNavigator;
        private readonly IPopupManager        _popupManager;
        private readonly ISpriteLoader        _spriteLoader;

        private SkillSO[]  _skillSoCache;
        private Sprite[]   _skillIconCache;

        public CharacterInfoPresenter(
            CharacterInfoUseCase useCase,
            CharacterInfoView    view,
            ISceneNavigator      sceneNavigator,
            IPopupManager        popupManager,
            ISpriteLoader        spriteLoader)
        {
            _useCase        = useCase;
            _view           = view;
            _sceneNavigator = sceneNavigator;
            _popupManager   = popupManager;
            _spriteLoader   = spriteLoader;
        }

        public void Initialize()
        {
            InitializeAsync().Forget();
        }

        private async UniTaskVoid InitializeAsync()
        {
            var evolutionNode = _useCase.GetCurrentEvolutionNode();

            // Character sprite — SpriteLoader로 로드
            var characterSprite = await _spriteLoader.LoadSpriteAsync(evolutionNode.MainStandingSpriteKey);
            _view.CharacterSpriteView.SetSprite(characterSprite);

            // Evolution stage button — 노드 아이콘 스프라이트 로드
            var evolutionNodeIcon = await _spriteLoader.LoadSpriteAsync(evolutionNode.NodeIconSpriteKey);
            _view.EvolutionStageButtonView.SetEvolutionInfo(evolutionNodeIcon, evolutionNode.CharacterName);

            // Stats
            var stats = _useCase.GetCurrentStats();
            _view.InfoScrollView.StatListView.SetStats(
                stats.Hp, stats.Strength, stats.Toughness, stats.Agility);

            // Character name
            _view.InfoScrollView.CharacterNameView.SetName(evolutionNode.CharacterName);

            // Skills — 아이콘 스프라이트를 캐시에 보관 (팝업에서 재사용)
            _skillSoCache   = _useCase.GetSkills();
            _skillIconCache = new Sprite[_skillSoCache.Length];
            var skillDisplayData = new SkillDisplayData[_skillSoCache.Length];
            for (int i = 0; i < _skillSoCache.Length; i++)
            {
                var iconSprite      = await _spriteLoader.LoadSpriteAsync(_skillSoCache[i].IconSpriteKey);
                _skillIconCache[i]  = iconSprite;
                skillDisplayData[i] = new SkillDisplayData
                {
                    Icon      = iconSprite,
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
            var icon = _skillIconCache != null && index < _skillIconCache.Length ? _skillIconCache[index] : null;
            _view.SkillDescriptionPopupView.Show(
                icon,
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
