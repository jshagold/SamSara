using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Samsara.Core.MasterData;
using UnityEngine;
using UnityEngine.UI;

namespace Samsara.Features.BattleScene.Presentation.QTE
{
    /// <summary>
    /// QTE 패널 뷰. v2.0.0: 슬라이드 인/아웃 + QTERingView 기반 링 판정.
    /// </summary>
    public class BattleQTEView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(BattleQTEView)}]";

        [SerializeField] private GameObject _qtePanel;
        [SerializeField] private Image _panelBackground;
        [SerializeField] private Image _panelBorder;
        [SerializeField] private QTERingView _ringView;

        [Header("Slide Animation")]
        [SerializeField] private float _slideDuration = 0.3f;

        [Header("Panel Colors")]
        [SerializeField] private Color _defaultBorderColor = new Color(0.6f, 0.6f, 0.6f, 1f);
        [SerializeField] private Color _defenseBorderColor = new Color(1f, 0.25f, 0.25f, 1f);

        private RectTransform _panelRect;
        private Vector2 _originalAnchoredPos;
        private const float SlideOffsetY = 1000f;

        private CancellationTokenSource _cts;

        private void Awake()
        {
            _panelRect = _qtePanel.GetComponent<RectTransform>();
            _originalAnchoredPos = _panelRect.anchoredPosition;
            _qtePanel.SetActive(false);
        }

        // ──────────────────────────────────────────────
        // Slide In / Out
        // ──────────────────────────────────────────────

        /// <summary>QTE 패널을 아래에서 슬라이드인. isDefense=true이면 방어 색상 보더.</summary>
        public async UniTask SlideIn(bool isDefense)
        {
            _panelBorder.color = isDefense ? _defenseBorderColor : _defaultBorderColor;
            _panelRect.anchoredPosition = _originalAnchoredPos + Vector2.down * SlideOffsetY;
            _qtePanel.SetActive(true);

            await _panelRect.DOAnchorPosY(_originalAnchoredPos.y, _slideDuration)
                .SetEase(Ease.OutQuad).AsyncWaitForCompletion();
        }

        /// <summary>QTE 패널을 아래로 슬라이드아웃 후 비활성화.</summary>
        public async UniTask SlideOut()
        {
            await _panelRect.DOAnchorPosY(_originalAnchoredPos.y - SlideOffsetY, _slideDuration)
                .SetEase(Ease.InQuad).AsyncWaitForCompletion();
            _qtePanel.SetActive(false);
            _panelRect.anchoredPosition = _originalAnchoredPos;
        }

        // ──────────────────────────────────────────────
        // Ring-based QTE
        // ──────────────────────────────────────────────

        /// <summary>
        /// 단일 QTE 입력을 QTERingView에 위임. Presenter가 per-hit으로 반복 호출.
        /// </summary>
        public async UniTask<bool> RunSingleRing(QTEData qteData, CancellationToken cancellationToken = default)
        {
            return await _ringView.RunRing(qteData, cancellationToken);
        }

        /// <summary>
        /// QTE 전체 시퀀스를 실행. 내부에서 QTERingView 반복 호출. bool[] 반환.
        /// SlideIn/SlideOut은 별도로 호출해야 한다.
        /// </summary>
        public async UniTask<bool[]> RunQTE(QTEData[] qteDataList, bool isAttack)
        {
            var results = new bool[qteDataList.Length];

            _cts?.Cancel();
            _cts?.Dispose();
            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            for (int i = 0; i < qteDataList.Length; i++)
            {
                results[i] = await _ringView.RunRing(qteDataList[i], token);

                if (i < qteDataList.Length - 1 && qteDataList[i].IntervalToNext > 0f)
                {
                    await UniTask.Delay(
                        (int)(qteDataList[i].IntervalToNext * 1000f),
                        cancellationToken: token
                    );
                }
            }

            return results;
        }

        public void Hide()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
            _qtePanel.SetActive(false);
        }

        private void OnDestroy()
        {
            _panelRect?.DOKill();
            _cts?.Cancel();
            _cts?.Dispose();
        }

        private void Reset()
        {
            _panelBackground = GetComponentInChildren<Image>();
            _ringView = GetComponentInChildren<QTERingView>();
        }
    }
}
