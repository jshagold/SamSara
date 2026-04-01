using DG.Tweening;
using UnityEngine;

namespace Samsara.Features.MiniGame.Presentation.Play
{
    public class MarkerView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(MarkerView)}]";

        [SerializeField] private RectTransform _rectTransform;

        private Tweener _tween;

        /// <summary>마커를 좌우로 왕복 이동시킨다.</summary>
        public void StartOscillation(float barWidth, float speed)
        {
            _rectTransform.anchoredPosition = Vector2.zero;
            float duration = barWidth / speed;
            _tween = _rectTransform
                .DOAnchorPosX(barWidth, duration)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Yoyo);
        }

        /// <summary>현재 위치에서 이동을 멈춘다.</summary>
        public void StopOscillation()
        {
            _tween?.Kill(false);
        }

        /// <summary>현재 마커 x 위치를 0~1로 정규화하여 반환한다.</summary>
        public float GetNormalizedPosition(float barWidth)
        {
            if (barWidth <= 0f) return 0f;
            return _rectTransform.anchoredPosition.x / barWidth;
        }

        /// <summary>마커를 시작 위치(0)로 초기화한다.</summary>
        public void ResetPosition()
        {
            _tween?.Kill(false);
            _rectTransform.anchoredPosition = Vector2.zero;
        }

        private void Reset()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        private void OnDestroy()
        {
            _tween?.Kill();
        }
    }
}
