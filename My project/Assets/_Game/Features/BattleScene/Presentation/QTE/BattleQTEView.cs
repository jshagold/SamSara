using System.Threading;
using Cysharp.Threading.Tasks;
using Samsara.Core.MasterData;
using UnityEngine;
using UnityEngine.UI;

namespace Samsara.Features.BattleScene.Presentation.QTE
{
    public class BattleQTEView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(BattleQTEView)}]";

        [SerializeField] private GameObject _qtePanel;
        [SerializeField] private RectTransform _touchArea;
        [SerializeField] private Image _targetIndicator;

        private bool _inputReceived;
        private bool _inputSuccess;
        private CancellationTokenSource _cts;

        public async UniTask<bool[]> RunQTE(QTEData[] qteDataList, bool isAttack)
        {
            _qtePanel.SetActive(true);
            var results = new bool[qteDataList.Length];

            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            for (int i = 0; i < qteDataList.Length; i++)
            {
                var qte = qteDataList[i];
                results[i] = await ProcessSingleQTE(qte, token);

                if (i < qteDataList.Length - 1 && qte.IntervalToNext > 0f)
                {
                    await UniTask.Delay(
                        (int)(qte.IntervalToNext * 1000f),
                        cancellationToken: token
                    );
                }
            }

            Hide();
            return results;
        }

        public void Hide()
        {
            _qtePanel.SetActive(false);
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
        }

        private async UniTask<bool> ProcessSingleQTE(QTEData qteData, CancellationToken token)
        {
            // Position target indicator at coordinate (0-1 ratio mapped to touch area)
            var areaSize = _touchArea.rect.size;
            _targetIndicator.rectTransform.anchoredPosition = new Vector2(
                (qteData.Coordinate.x - 0.5f) * areaSize.x,
                (qteData.Coordinate.y - 0.5f) * areaSize.y
            );
            _targetIndicator.gameObject.SetActive(true);

            _inputReceived = false;
            _inputSuccess = false;

            float elapsed = 0f;
            float duration = qteData.Duration;

            while (elapsed < duration && !token.IsCancellationRequested)
            {
                if (CheckTouchInput(qteData))
                {
                    _inputReceived = true;
                    _inputSuccess = true;
                    break;
                }

                elapsed += Time.deltaTime;
                await UniTask.Yield(token);
            }

            _targetIndicator.gameObject.SetActive(false);

            // If no input received within duration, it's a miss
            return _inputSuccess;
        }

        private bool CheckTouchInput(QTEData qteData)
        {
            if (!Input.GetMouseButtonDown(0) && (Input.touchCount == 0 || Input.GetTouch(0).phase != TouchPhase.Began))
                return false;

            Vector2 inputPos;
            if (Input.touchCount > 0)
                inputPos = Input.GetTouch(0).position;
            else
                inputPos = Input.mousePosition;

            // Convert screen position to local position in touch area
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _touchArea, inputPos, null, out var localPoint
            );

            // Check distance from target
            var targetPos = _targetIndicator.rectTransform.anchoredPosition;
            float distance = Vector2.Distance(localPoint, targetPos);
            float threshold = _targetIndicator.rectTransform.rect.width * 0.5f;

            return distance <= threshold;
        }

        private void OnDestroy()
        {
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
        }

        private void Reset()
        {
            _targetIndicator = GetComponentInChildren<Image>();
        }
    }
}
