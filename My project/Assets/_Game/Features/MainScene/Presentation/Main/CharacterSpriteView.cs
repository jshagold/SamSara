using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace Samsara.Features.MainScene.Presentation.Main
{
    public class CharacterSpriteView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(CharacterSpriteView)}]";

        [SerializeField] private Image _characterImage;

        private AsyncOperationHandle<Sprite> _spriteHandle;

        public async UniTask SetSprite(string addressableKey)
        {
            ReleaseHandle();

            _spriteHandle = Addressables.LoadAssetAsync<Sprite>(addressableKey);
            var sprite = await _spriteHandle.ToUniTask();
            _characterImage.sprite = sprite;
        }

        private void ReleaseHandle()
        {
            if (_spriteHandle.IsValid())
            {
                Addressables.Release(_spriteHandle);
            }
        }

        private void OnDestroy()
        {
            ReleaseHandle();
        }

        private void Reset()
        {
            _characterImage = GetComponentInChildren<Image>();
        }
    }
}
