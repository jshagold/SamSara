using System;
using UnityEngine;

namespace Samsara.Features.Event.Presentation
{
    public class EventView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(EventView)}]";

        [SerializeField] private BackgroundView          _backgroundView;
        [SerializeField] private DialogueView            _dialogueView;
        [SerializeField] private ChoiceListView          _choiceListView;
        [SerializeField] private ChainStageIndicatorView _chainStageIndicatorView;
        [SerializeField] private EventResultPopupView    _eventResultPopupView;
        [SerializeField] private OptionButtonView        _optionButtonView;

        public BackgroundView          BackgroundView          => _backgroundView;
        public DialogueView            DialogueView            => _dialogueView;
        public ChoiceListView          ChoiceListView          => _choiceListView;
        public ChainStageIndicatorView ChainStageIndicatorView => _chainStageIndicatorView;
        public EventResultPopupView    EventResultPopupView    => _eventResultPopupView;
        public OptionButtonView        OptionButtonView        => _optionButtonView;

        /// <summary>화면 탭 감지 이벤트. Update()에서 발생, 대사 진행에 사용.</summary>
        public event Action OnScreenTapped;

        private void Update()
        {
            // GC 최적화: Update 내 new/LINQ 금지 (Constitution §8)
            if (Input.GetMouseButtonDown(0))
            {
                OnScreenTapped?.Invoke();
                return;
            }
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                OnScreenTapped?.Invoke();
            }
        }

        private void Reset()
        {
            _backgroundView          = GetComponentInChildren<BackgroundView>();
            _dialogueView            = GetComponentInChildren<DialogueView>();
            _choiceListView          = GetComponentInChildren<ChoiceListView>();
            _chainStageIndicatorView = GetComponentInChildren<ChainStageIndicatorView>();
            _eventResultPopupView    = GetComponentInChildren<EventResultPopupView>();
            _optionButtonView        = GetComponentInChildren<OptionButtonView>();
        }
    }
}
