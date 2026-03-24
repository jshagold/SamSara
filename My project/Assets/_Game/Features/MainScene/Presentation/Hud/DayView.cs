using TMPro;
using UnityEngine;

namespace Samsara.Features.MainScene.Presentation.Hud
{
    public class DayView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(DayView)}]";

        [SerializeField] private TMP_Text _dayText;

        public void SetDay(int day)
        {
            _dayText.text = $"Day {day}";
        }

        private void Reset()
        {
            _dayText = GetComponentInChildren<TMP_Text>();
        }
    }
}
