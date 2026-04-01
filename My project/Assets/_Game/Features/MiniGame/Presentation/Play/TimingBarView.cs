using UnityEngine;
using UnityEngine.UI;

namespace Samsara.Features.MiniGame.Presentation.Play
{
    public class TimingBarView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(TimingBarView)}]";

        [SerializeField] private Image _barImage;
        // NOTE: RectTransform은 Reset() 자동 할당 제외 — Inspector에서 수동 연결 필요
        [SerializeField] private RectTransform _successZone;

        /// <summary>성공 구간의 위치와 크기를 정규화된 좌표(0~1)로 설정한다.</summary>
        public void SetSuccessZone(float normalizedStart, float normalizedEnd)
        {
            float barWidth = GetBarWidth();
            float startX   = normalizedStart * barWidth;
            float zoneWidth = (normalizedEnd - normalizedStart) * barWidth;

            _successZone.anchoredPosition = new Vector2(startX, _successZone.anchoredPosition.y);
            _successZone.sizeDelta        = new Vector2(zoneWidth, _successZone.sizeDelta.y);
        }

        /// <summary>타이밍 바 RectTransform의 너비를 반환한다.</summary>
        public float GetBarWidth()
        {
            return ((RectTransform)transform).rect.width;
        }

        private void Reset()
        {
            _barImage = GetComponentInChildren<Image>();
            // _successZone: RectTransform은 자동 할당 제외 (§7 예외)
        }
    }
}
