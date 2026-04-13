using UnityEngine;
using UnityEngine.UI;

namespace Samsara.Features.Event.Presentation
{
    public class PortraitView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(PortraitView)}]";

        [SerializeField] private Image       _portraitImage;
        [SerializeField] private CanvasGroup _dimOverlay;

        public void SetPortrait(Sprite sprite)
        {
            _portraitImage.sprite = sprite;
            _portraitImage.enabled = sprite != null;
        }

        public void SetHighlight(bool isActive)
        {
            _dimOverlay.alpha = isActive ? 0f : 0.5f;
        }

        public void Show() => gameObject.SetActive(true);
        public void Hide() => gameObject.SetActive(false);

        private void Reset()
        {
            _portraitImage = GetComponentInChildren<Image>();
            _dimOverlay    = GetComponentInChildren<CanvasGroup>();
        }
    }
}
