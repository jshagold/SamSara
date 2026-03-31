using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Samsara.Features.StageScene.Presentation.TopBar
{
    public class DayView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(DayView)}]";

        [SerializeField] private TMP_Text _dayText;

        public void SetDay(int day)
        {
            _dayText.text = $"Day {day}";
            _dayText.transform.DOPunchScale(Vector3.one * 0.2f, 0.3f, 5, 0.5f);
        }
    }
}
