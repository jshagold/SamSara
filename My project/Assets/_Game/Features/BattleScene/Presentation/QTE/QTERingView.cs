using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Samsara.Core.MasterData;
using UnityEngine;
using UnityEngine.UI;

namespace Samsara.Features.BattleScene.Presentation.QTE
{
    /// <summary>
    /// 개별 QTE 입력 — 링이 외곽에서 중심으로 수렴하는 클로징 링 애니메이션.
    /// 링 스케일이 성공 범위 안에 있을 때 _buttonImage 영역 내 터치하면 성공.
    /// Duration이 경과하면 자동으로 실패 처리.
    /// </summary>
    public class QTERingView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(QTERingView)}]";

        [SerializeField] private Image _buttonImage;
        [SerializeField] private Image _ringImage;
        [SerializeField] private RectTransform _ringRect;

        // 링 시작 스케일 (1.0 = 버튼 크기와 동일)
        private const float RingStartScale = 3.0f;
        // 이 범위 안에서 터치하면 성공
        private const float SuccessRangeMin = 1.0f;
        private const float SuccessRangeMax = 1.3f;

        // 부모 Canvas 카메라 — RectTransformUtility에 전달 (ScreenSpaceOverlay = null)
        private Camera _canvasCamera;

        private void Awake()
        {
            var canvas = GetComponentInParent<Canvas>();
            _canvasCamera = (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
                ? canvas.worldCamera
                : null;

            gameObject.SetActive(false);
        }

        /// <summary>
        /// QTE 링 하나를 실행하고 성공 여부를 반환한다.
        /// 1. 좌표에 배치 → 2. 링이 RingStartScale → 1.0으로 수렴
        /// → 3. _buttonImage 영역 내 터치 판정 → 4. Duration 경과 시 자동 실패
        /// </summary>
        public async UniTask<bool> RunRing(QTEData qteData, CancellationToken cancellationToken = default)
        {
            // 부모 rect 기준으로 좌표 배치
            PositionAtCoordinate(qteData.Coordinate);

            // 링 초기화
            _ringRect.localScale = Vector3.one * RingStartScale;
            gameObject.SetActive(true);

            // DOTween으로 링 수렴 애니메이션 시작
            var tween = _ringRect.DOScale(Vector3.one, qteData.Duration)
                .SetEase(Ease.Linear);

            bool success = false;
            float elapsed = 0f;

            // Duration 타임아웃 기반 루프 — tween.IsComplete() 대신 사용
            // 이유: DOTween SetAutoKill(true, 기본값)으로 완료 후 tween 재활용됨,
            //       재활용된 객체의 IsComplete() 는 정의되지 않은 동작.
            while (elapsed < qteData.Duration && !cancellationToken.IsCancellationRequested)
            {
                if (HasTouchBeganOnButton())
                {
                    float currentScale = _ringRect.localScale.x;
                    success = currentScale >= SuccessRangeMin && currentScale <= SuccessRangeMax;
                    break;
                }

                await UniTask.Yield(cancellationToken: cancellationToken);
                elapsed += Time.deltaTime;
            }
            // elapsed >= duration 으로 루프 종료 시 success = false (자동 실패)

            tween.Kill();
            gameObject.SetActive(false);

            return success;
        }

        private void PositionAtCoordinate(Vector2 coordinate)
        {
            if (transform.parent == null) return;

            var parentRect = ((RectTransform)transform.parent).rect;
            var rt = (RectTransform)transform;
            rt.anchoredPosition = new Vector2(
                (coordinate.x - 0.5f) * parentRect.width,
                (coordinate.y - 0.5f) * parentRect.height
            );
        }

        /// <summary>
        /// _buttonImage RectTransform 영역 내 터치/클릭 여부 반환.
        /// RectTransformUtility.RectangleContainsScreenPoint으로 버튼 밖 입력 차단.
        /// </summary>
        private bool HasTouchBeganOnButton()
        {
            Vector2 screenPoint;
            if (Input.GetMouseButtonDown(0))
                screenPoint = Input.mousePosition;
            else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
                screenPoint = Input.GetTouch(0).position;
            else
                return false;

            return RectTransformUtility.RectangleContainsScreenPoint(
                _buttonImage.rectTransform, screenPoint, _canvasCamera);
        }

        private void OnDestroy()
        {
            _ringRect?.DOKill();
        }

        private void Reset()
        {
            _ringImage = GetComponentInChildren<Image>();
            _ringRect = _ringImage != null ? _ringImage.rectTransform : GetComponent<RectTransform>();
        }
    }
}
