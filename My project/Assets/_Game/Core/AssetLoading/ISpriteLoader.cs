using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Samsara.Core.AssetLoading
{
    public interface ISpriteLoader
    {
        /// <summary>단일 Sprite를 비동기 로드. 실패 시 null 반환.</summary>
        UniTask<Sprite> LoadSpriteAsync(string key);

        /// <summary>여러 Sprite를 일괄 사전 로드.</summary>
        UniTask PreloadSpritesAsync(string[] keys);

        /// <summary>특정 Sprite 캐시 해제. Phase 1: no-op.</summary>
        void ReleaseSpriteAsync(string key);

        /// <summary>모든 캐시된 Sprite 해제. Phase 1: no-op.</summary>
        void ReleaseAllAsync();
    }
}
