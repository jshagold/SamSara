using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Samsara.Features.StageScene.Presentation.Map
{
    public class CharacterMarkerView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(CharacterMarkerView)}]";

        [SerializeField] private Image _characterSprite;

        public void SetSprite(Sprite sprite)
        {
            _characterSprite.sprite = sprite;
        }

        public async UniTask MoveTo(Vector3 worldPosition, float duration)
        {
            await transform.DOMove(worldPosition, duration).SetEase(Ease.OutCubic).ToUniTask();
        }

        public void SetPosition(Vector3 worldPosition)
        {
            transform.position = worldPosition;
        }
    }
}
