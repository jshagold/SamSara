using Samsara.Features.MiniGame.Presentation.Play;
using Samsara.Features.MiniGame.Presentation.Result;
using Samsara.Features.MiniGame.Presentation.TopBar;
using UnityEngine;

namespace Samsara.Features.MiniGame.Presentation
{
    public class MiniGameView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(MiniGameView)}]";

        [SerializeField] private GuideTooltipView        _guideTooltipView;
        [SerializeField] private TimingBarView            _timingBarView;
        [SerializeField] private MarkerView               _markerView;
        [SerializeField] private TouchButtonView          _touchButtonView;
        [SerializeField] private RoundIndicatorView       _roundIndicatorView;
        [SerializeField] private OptionButtonView         _optionButtonView;
        [SerializeField] private MiniGameResultPopupView  _miniGameResultPopupView;

        public GuideTooltipView       GuideTooltip => _guideTooltipView;
        public TimingBarView          TimingBar    => _timingBarView;
        public MarkerView             Marker       => _markerView;
        public TouchButtonView        TouchButton  => _touchButtonView;
        public RoundIndicatorView     RoundIndicator => _roundIndicatorView;
        public OptionButtonView       OptionButton => _optionButtonView;
        public MiniGameResultPopupView ResultPopup => _miniGameResultPopupView;

        private void Reset()
        {
            _guideTooltipView       = GetComponentInChildren<GuideTooltipView>();
            _timingBarView          = GetComponentInChildren<TimingBarView>();
            _markerView             = GetComponentInChildren<MarkerView>();
            _touchButtonView        = GetComponentInChildren<TouchButtonView>();
            _roundIndicatorView     = GetComponentInChildren<RoundIndicatorView>();
            _optionButtonView       = GetComponentInChildren<OptionButtonView>();
            // true: 비활성 오브젝트(ResultPopup 초기 비활성 상태) 포함 탐색
            _miniGameResultPopupView = GetComponentInChildren<MiniGameResultPopupView>(true);
        }
    }
}
