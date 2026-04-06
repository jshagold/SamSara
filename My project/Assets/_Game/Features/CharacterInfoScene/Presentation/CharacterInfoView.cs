using Samsara.Features.CharacterInfoScene.Presentation.CharacterArea;
using Samsara.Features.CharacterInfoScene.Presentation.InfoScroll;
using Samsara.Features.CharacterInfoScene.Presentation.Popup;
using Samsara.Features.CharacterInfoScene.Presentation.TopBar;
using UnityEngine;

namespace Samsara.Features.CharacterInfoScene.Presentation
{
    public class CharacterInfoView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(CharacterInfoView)}]";

        [SerializeField] private BackButtonView             _backButtonView;
        [SerializeField] private OptionButtonView           _optionButtonView;
        [SerializeField] private CharacterSpriteView        _characterSpriteView;
        [SerializeField] private EvolutionStageButtonView   _evolutionStageButtonView;
        [SerializeField] private InfoScrollView             _infoScrollView;
        [SerializeField] private SkillDescriptionPopupView  _skillDescriptionPopupView;

        public BackButtonView            BackButtonView            => _backButtonView;
        public OptionButtonView          OptionButtonView          => _optionButtonView;
        public CharacterSpriteView       CharacterSpriteView       => _characterSpriteView;
        public EvolutionStageButtonView  EvolutionStageButtonView  => _evolutionStageButtonView;
        public InfoScrollView            InfoScrollView            => _infoScrollView;
        public SkillDescriptionPopupView SkillDescriptionPopupView => _skillDescriptionPopupView;

        private void Reset()
        {
            _backButtonView            = GetComponentInChildren<BackButtonView>();
            _optionButtonView          = GetComponentInChildren<OptionButtonView>();
            _characterSpriteView       = GetComponentInChildren<CharacterSpriteView>();
            _evolutionStageButtonView  = GetComponentInChildren<EvolutionStageButtonView>();
            _infoScrollView            = GetComponentInChildren<InfoScrollView>();
            _skillDescriptionPopupView = GetComponentInChildren<SkillDescriptionPopupView>();
        }
    }
}
