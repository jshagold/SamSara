using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Samsara.Features.BattleScene.Presentation.Result
{
    /// <summary>
    /// 전투 시작 연출. "Battle Start!" 텍스트를 페이드인 → 홀드 → 페이드아웃 순으로 재생.
    /// </summary>
    public class BattleStartView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(BattleStartView)}]";

        [SerializeField] private TMP_Text _battleStartText;
        [SerializeField] private CanvasGroup _canvasGroup;

        private void Awake()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// 전투 시작 연출을 재생하고 완료까지 기다린다.
        /// Scale up + Fade in (~0.5s) → Hold (~0.5s) → Fade out (~0.3s) → 비활성화
        /// </summary>
        public async UniTask PlayStartPresentation()
        {
            _battleStartText.text = "Battle Start!";
            _canvasGroup.alpha = 0f;
            _battleStartText.transform.localScale = Vector3.one * 0.5f;

            gameObject.SetActive(true);

            // Fade in + Scale up
            var seq = DOTween.Sequence();
            seq.Append(_canvasGroup.DOFade(1f, 0.5f).SetEase(Ease.OutQuad));
            seq.Join(_battleStartText.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack));
            await seq.AsyncWaitForCompletion();

            // Hold
            await UniTask.Delay(500);

            // Fade out
            await _canvasGroup.DOFade(0f, 0.3f).SetEase(Ease.InQuad).AsyncWaitForCompletion();

            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            _canvasGroup?.DOKill();
            _battleStartText?.transform.DOKill();
        }

        private void Reset()
        {
            _battleStartText = GetComponentInChildren<TMP_Text>();
            _canvasGroup = GetComponentInChildren<CanvasGroup>();
        }
    }
}
