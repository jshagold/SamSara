using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Samsara.Features.StageScene.Presentation.Background
{
    public class BackgroundView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(BackgroundView)}]";

        [SerializeField] private Image _backgroundImage;

        public void SetBackground(Sprite sprite)
        {
            _backgroundImage.sprite = sprite;
        }

        public void FadeToBackground(Sprite sprite, float duration)
        {
            _backgroundImage.DOFade(0f, duration * 0.5f)
                .OnComplete(() =>
                {
                    _backgroundImage.sprite = sprite;
                    _backgroundImage.DOFade(1f, duration * 0.5f);
                });
        }
    }
}
