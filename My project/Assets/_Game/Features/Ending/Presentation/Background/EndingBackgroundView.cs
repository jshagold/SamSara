using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Samsara.Features.Ending.Presentation
{
    public class EndingBackgroundView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(EndingBackgroundView)}]";

        [SerializeField] private Image _backgroundImage;

        public void SetBackground(Sprite sprite)
        {
            _backgroundImage.sprite = sprite;
            var c = _backgroundImage.color;
            _backgroundImage.color = new Color(c.r, c.g, c.b, 1f);
        }

        /// <summary>
        /// 현재 배경을 페이드아웃 → 스프라이트 교체 → 페이드인.
        /// </summary>
        public async UniTask FadeToBackground(Sprite sprite, float duration)
        {
            float half = duration * 0.5f;
            await _backgroundImage.DOFade(0f, half).ToUniTask();
            _backgroundImage.sprite = sprite;
            await _backgroundImage.DOFade(1f, half).ToUniTask();
        }

        private void Reset()
        {
            _backgroundImage = GetComponentInChildren<Image>();
        }

        private void OnDestroy()
        {
            _backgroundImage?.DOKill();
        }
    }
}
