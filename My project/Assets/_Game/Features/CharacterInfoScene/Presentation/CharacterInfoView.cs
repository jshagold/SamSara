using Samsara.Features.CharacterInfoScene.Presentation.CharacterArea;
using Samsara.Features.CharacterInfoScene.Presentation.InfoScroll;
using Samsara.Features.CharacterInfoScene.Presentation.Popup;
using Samsara.Features.CharacterInfoScene.Presentation.TopBar;
using UnityEngine;


namespace Samsara.Features.CharacterInfoScene.Presentation
{
    /// <summary>
    /// CharacterInfoScene 루트 View. 모든 하위 View를 통합한다.
    /// </summary>
    public class CharacterInfoView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(CharacterInfoView)}]";

        [SerializeField] private BackButtonView           _backButtonView;
        [SerializeField] private OptionButtonView         _optionButtonView;
        [SerializeField] private CharacterSpriteView      _characterSpriteView;
        [SerializeField] private EvolutionStageButtonView _evolutionStageButtonView;
        [SerializeField] private InfoScrollView           _infoScrollView;
        [SerializeField] private SkillDescriptionPopupView _skillDescriptionPopupView;
        [SerializeField] private ItemDetailPopupView       _itemDetailPopupView;

        // ── Child View 프로퍼티 (Presenter 접근용) ──
        public BackButtonView            BackButton            => _backButtonView;
        public OptionButtonView          OptionButton          => _optionButtonView;
        public CharacterSpriteView       CharacterSprite       => _characterSpriteView;
        public EvolutionStageButtonView  EvolutionStageButton  => _evolutionStageButtonView;
        public InfoScrollView            InfoScroll            => _infoScrollView;
        public SkillDescriptionPopupView SkillDescriptionPopup => _skillDescriptionPopupView;
        public ItemDetailPopupView       ItemDetailPopup       => _itemDetailPopupView;

        private void Reset()
        {
            _backButtonView            = GetComponentInChildren<BackButtonView>();
            _optionButtonView          = GetComponentInChildren<OptionButtonView>();
            _characterSpriteView       = GetComponentInChildren<CharacterSpriteView>();
            _evolutionStageButtonView  = GetComponentInChildren<EvolutionStageButtonView>();
            _infoScrollView            = GetComponentInChildren<InfoScrollView>();
            _skillDescriptionPopupView = GetComponentInChildren<SkillDescriptionPopupView>();
            _itemDetailPopupView       = GetComponentInChildren<ItemDetailPopupView>();
        }
    }
}
