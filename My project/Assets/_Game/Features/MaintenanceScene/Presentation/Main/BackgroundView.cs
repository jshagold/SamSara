using UnityEngine;
using UnityEngine.UI;

namespace Samsara.Features.MaintenanceScene.Presentation.Main
{
    public class BackgroundView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(BackgroundView)}]";

        [SerializeField] private Image _backgroundImage;

        private void Reset()
        {
            _backgroundImage = GetComponentInChildren<Image>();
        }
    }
}
