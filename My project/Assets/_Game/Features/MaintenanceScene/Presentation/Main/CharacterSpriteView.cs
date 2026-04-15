using UnityEngine;
using UnityEngine.UI;

namespace Samsara.Features.MaintenanceScene.Presentation.Main
{
    public class CharacterSpriteView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(CharacterSpriteView)}]";

        [SerializeField] private Image _characterImage;

        public void SetSprite(Sprite sprite)
        {
            _characterImage.sprite = sprite;
        }

        private void Reset()
        {
            _characterImage = GetComponentInChildren<Image>();
        }
    }
}
