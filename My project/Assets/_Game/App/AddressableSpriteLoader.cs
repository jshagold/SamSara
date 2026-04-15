using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Samsara.Core.AssetLoading;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Samsara.App
{
    /// <summary>
    /// Addressables 기반 Sprite 로더. 로드된 Sprite를 키별로 캐시한다.
    /// </summary>
    public class AddressableSpriteLoader : ISpriteLoader
    {
        private readonly string _logClass = $"[{nameof(AddressableSpriteLoader)}]";

        private readonly Dictionary<string, Sprite> _cache = new Dictionary<string, Sprite>();

        public async UniTask<Sprite> LoadSpriteAsync(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                Debug.LogWarning($"{_logClass} LoadSpriteAsync: key is null or empty");
                return null;
            }

            if (_cache.TryGetValue(key, out var cached))
                return cached;

            try
            {
                var sprite = await Addressables.LoadAssetAsync<Sprite>(key).ToUniTask();
                _cache[key] = sprite;
                return sprite;
            }
            catch (Exception e)
            {
                Debug.LogError($"{_logClass} LoadSpriteAsync failed for key: {key} -- {e.Message}");
                return null;
            }
        }

        public async UniTask PreloadSpritesAsync(string[] keys)
        {
            if (keys == null || keys.Length == 0) return;

            var tasks = new UniTask[keys.Length];
            for (int i = 0; i < keys.Length; i++)
                tasks[i] = LoadSpriteAsync(keys[i]);

            await UniTask.WhenAll(tasks);
        }

        /// <summary>Phase 1: no-op. Future implementation for reference counting.</summary>
        public void ReleaseSpriteAsync(string key) { }

        /// <summary>Phase 1: no-op. Future implementation for reference counting.</summary>
        public void ReleaseAllAsync() { }
    }
}
