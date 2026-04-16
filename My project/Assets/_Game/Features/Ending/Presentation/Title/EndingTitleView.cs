using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Samsara.Features.Ending.Presentation
{
    public class EndingTitleView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(EndingTitleView)}]";

        [SerializeField] private TMP_Text    _titleText;
        [SerializeField] private CanvasGroup _canvasGroup;

        /// <summary>
        /// 타이틀 텍스트를 페이드인 → 유지 → 페이드아웃으로 표시한다.
        /// </summary>
        public async UniTask ShowTitle(string title, float fadeInDuration, float holdDuration, float fadeOutDuration)
        {
            _titleText.text   = title;
            _canvasGroup.alpha = 0f;
            gameObject.SetActive(true);

            await _canvasGroup.DOFade(1f, fadeInDuration).ToUniTask();
            await UniTask.Delay((int)(holdDuration * 1000f));
            await _canvasGroup.DOFade(0f, fadeOutDuration).ToUniTask();

            gameObject.SetActive(false);
        }

        private void Reset()
        {
            _titleText    = GetComponentInChildren<TMP_Text>();
            _canvasGroup  = GetComponent<CanvasGroup>();
        }

        private void OnDestroy()
        {
            _canvasGroup?.DOKill();
        }
    }
}
