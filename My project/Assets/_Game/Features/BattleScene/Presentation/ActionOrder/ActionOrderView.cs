using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

namespace Samsara.Features.BattleScene.Presentation.ActionOrder
{
    public struct ActionOrderEntry
    {
        public int Id;
        public string SpriteKey;
        public bool IsAlly;
    }

    public class ActionOrderView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(ActionOrderView)}]";

        [SerializeField] private Transform _iconContainer;
        [SerializeField] private Image _iconPrefab;

        private ObjectPool<Image> _iconPool;
        private readonly List<Image> _activeIcons = new List<Image>();
        private readonly Dictionary<int, Image> _iconById = new Dictionary<int, Image>();

        private void Awake()
        {
            _iconPool = new ObjectPool<Image>(
                createFunc: () => Instantiate(_iconPrefab, _iconContainer),
                actionOnGet: icon => icon.gameObject.SetActive(true),
                actionOnRelease: icon => icon.gameObject.SetActive(false),
                actionOnDestroy: icon => Destroy(icon.gameObject),
                defaultCapacity: 8,
                maxSize: 20
            );
        }

        public void SetOrder(ActionOrderEntry[] entries)
        {
            ClearOrder();

            foreach (var entry in entries)
            {
                var icon = _iconPool.Get();
                icon.color = entry.IsAlly ? Color.cyan : Color.red;
                _activeIcons.Add(icon);
                _iconById[entry.Id] = icon;
            }
        }

        public void HighlightCurrent(int id)
        {
            foreach (var kvp in _iconById)
            {
                var icon = kvp.Value;
                icon.transform.localScale = kvp.Key == id ? Vector3.one * 1.2f : Vector3.one;
            }
        }

        public void ClearOrder()
        {
            foreach (var icon in _activeIcons)
                _iconPool.Release(icon);

            _activeIcons.Clear();
            _iconById.Clear();
        }

        private void OnDestroy()
        {
            _iconPool?.Dispose();
        }

        private void Reset()
        {
            _iconContainer = transform;
        }
    }
}
