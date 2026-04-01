using TMPro;
using UnityEngine;

namespace Samsara.Features.MiniGame.Presentation.Play
{
    public class GuideTooltipView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(GuideTooltipView)}]";

        [SerializeField] private TMP_Text _guideText;

        public void SetText(string text)
        {
            _guideText.text = text;
        }

        private void Reset()
        {
            _guideText = GetComponentInChildren<TMP_Text>();
        }
    }
}
