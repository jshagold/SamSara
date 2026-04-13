using TMPro;
using UnityEngine;

namespace Samsara.Features.Event.Presentation
{
    public class ChainStageIndicatorView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(ChainStageIndicatorView)}]";

        [SerializeField] private TMP_Text _indicatorText;

        public void Show(string eventName, int step, int total)
        {
            gameObject.SetActive(true);
            _indicatorText.text = $"({eventName} - {step}/{total})";
        }

        public void Hide() => gameObject.SetActive(false);

        private void Reset()
        {
            _indicatorText = GetComponentInChildren<TMP_Text>();
        }
    }
}
