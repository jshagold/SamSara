using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Samsara.Features.Ending.Presentation
{
    public class EndingView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(EndingView)}]";

        [SerializeField] private EndingBackgroundView   _backgroundView;
        [SerializeField] private EndingTitleView        _titleView;
        [SerializeField] private EndingDialogueView     _dialogueView;
        [SerializeField] private EndingResultPopupView  _resultPopupView;
        [SerializeField] private OptionButtonView       _optionButtonView;
        [SerializeField] private SkipButtonView         _skipButtonView;

        public EndingBackgroundView  BackgroundView    => _backgroundView;
        public EndingTitleView       TitleView         => _titleView;
        public EndingDialogueView    DialogueView      => _dialogueView;
        public EndingResultPopupView ResultPopupView   => _resultPopupView;
        public OptionButtonView      OptionButtonView  => _optionButtonView;
        public SkipButtonView        SkipButtonView    => _skipButtonView;

        // ── 탭 대기 TCS (Update → WaitForTapAsync 연결) ──
        private UniTaskCompletionSource _tapTcs;

        /// <summary>
        /// 화면 탭(터치) 한 번을 UniTask로 대기한다.
        /// EventView.OnScreenTapped 패턴과 동일한 방식. GC 최적화: new는 호출당 1회.
        /// </summary>
        public UniTask WaitForTapAsync()
        {
            _tapTcs = new UniTaskCompletionSource();
            return _tapTcs.Task;
        }

        private void Update()
        {
            if (_tapTcs == null) return;

            // GC 최적화: Update 내 new/LINQ 금지 (Constitution §8)
            if (Input.GetMouseButtonDown(0))
            {
                _tapTcs.TrySetResult();
                _tapTcs = null;
                return;
            }
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                _tapTcs.TrySetResult();
                _tapTcs = null;
            }
        }

        private void OnDestroy()
        {
            _tapTcs?.TrySetCanceled();
        }

        private void Reset()
        {
            _backgroundView   = GetComponentInChildren<EndingBackgroundView>();
            _titleView        = GetComponentInChildren<EndingTitleView>();
            _dialogueView     = GetComponentInChildren<EndingDialogueView>();
            _resultPopupView  = GetComponentInChildren<EndingResultPopupView>();
            _optionButtonView = GetComponentInChildren<OptionButtonView>();
            _skipButtonView   = GetComponentInChildren<SkipButtonView>();
        }
    }
}
