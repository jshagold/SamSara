using UnityEngine;
using UnityEngine.UI;

namespace Samsara.Features.MaintenanceScene.Presentation.Main
{
    public class BackgroundView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(BackgroundView)}]";

        [SerializeField] private Image _backgroundImage;

        public void SetBackground(Sprite sprite)
        {
            _backgroundImage.sprite = sprite;
        }

        private void Reset()
        {
            _backgroundImage = GetComponentInChildren<Image>();
        }
    }
}
