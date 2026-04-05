using System.Collections.Generic;
using TMPro;
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
        public bool IsCurrent;
        public int TurnNumber;
    }

    public class ActionOrderView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(ActionOrderView)}]";

        [SerializeField] private Transform _iconContainer;
        [SerializeField] private Image _iconPrefab;

        [Header("Icon Size")]
        [SerializeField] private float _currentIconSize = 90f;
        [SerializeField] private float _upcomingIconSize = 56f;

        [Header("Border Colors")]
        [SerializeField] private Color _allyBorderColor = new Color(0.2f, 0.5f, 1f, 1f);
        [SerializeField] private Color _enemyBorderColor = new Color(1f, 0.3f, 0.3f, 1f);

        [Header("Border Thickness")]
        [SerializeField] private Vector2 _currentBorderSize = new Vector2(3f, 3f);
        [SerializeField] private Vector2 _upcomingBorderSize = new Vector2(2f, 2f);

        [Header("Layout")]
        [SerializeField] private float _iconSpacing = 6f;
        [SerializeField] private int _maxDisplayCount = 5;

        [Header("Turn Label")]
        [SerializeField] private float _labelFontSizeCurrent = 18f;
        [SerializeField] private float _labelFontSizeUpcoming = 14f;

        private ObjectPool<Image> _iconPool;
        private readonly List<Image> _activeIcons = new List<Image>();

        private void Awake()
        {
            ConfigureLayout();

            _iconPool = new ObjectPool<Image>(
                createFunc: CreateIcon,
                actionOnGet: icon => icon.gameObject.SetActive(true),
                actionOnRelease: icon => icon.gameObject.SetActive(false),
                actionOnDestroy: icon => Destroy(icon.gameObject),
                defaultCapacity: _maxDisplayCount,
                maxSize: 12
            );
        }

        private void ConfigureLayout()
        {
            var vlg = _iconContainer.GetComponent<VerticalLayoutGroup>();
            if (vlg == null)
                vlg = _iconContainer.gameObject.AddComponent<VerticalLayoutGroup>();

            vlg.childAlignment = TextAnchor.UpperLeft;
            vlg.spacing = _iconSpacing;
            vlg.childForceExpandWidth = false;
            vlg.childForceExpandHeight = false;
            vlg.childControlWidth = true;
            vlg.childControlHeight = true;
            vlg.childScaleWidth = false;
            vlg.childScaleHeight = false;
        }

        private Image CreateIcon()
        {
            var icon = Instantiate(_iconPrefab, _iconContainer);

            if (icon.gameObject.GetComponent<LayoutElement>() == null)
                icon.gameObject.AddComponent<LayoutElement>();

            if (icon.gameObject.GetComponent<Outline>() == null)
                icon.gameObject.AddComponent<Outline>();

            return icon;
        }

        public void SetOrder(ActionOrderEntry[] entries)
        {
            ClearOrder();

            int count = Mathf.Min(entries.Length, _maxDisplayCount);

            for (int i = 0; i < count; i++)
            {
                var entry = entries[i];
                var icon = _iconPool.Get();
                icon.transform.SetAsLastSibling();
                bool isCurrent = entry.IsCurrent;

                // ── Size via LayoutElement ──
                float size = isCurrent ? _currentIconSize : _upcomingIconSize;
                var le = icon.GetComponent<LayoutElement>();
                le.preferredWidth = size;
                le.preferredHeight = size;

                // ── Sprite ──
                LoadSprite(icon, entry.SpriteKey);

                // ── Color: same base per team, alpha differentiates current/upcoming ──
                float alpha = isCurrent ? 1f : 0.65f;
                if (icon.sprite == null)
                {
                    icon.color = entry.IsAlly
                        ? new Color(0.5f, 0.75f, 1f, alpha)
                        : new Color(1f, 0.55f, 0.3f, alpha);
                }
                else
                {
                    icon.color = new Color(1f, 1f, 1f, alpha);
                }

                // ── Border outline ──
                var outline = icon.GetComponent<Outline>();
                outline.effectColor = entry.IsAlly ? _allyBorderColor : _enemyBorderColor;
                outline.effectDistance = isCurrent ? _currentBorderSize : _upcomingBorderSize;
                outline.enabled = true;

                // ── Turn label (TMP_Text is set up in prefab via Editor) ──
                var label = icon.GetComponentInChildren<TMP_Text>(true);
                if (label != null)
                {
                    label.text = $"Turn {entry.TurnNumber}";
                    label.fontSize = isCurrent ? _labelFontSizeCurrent : _labelFontSizeUpcoming;
                    label.color = isCurrent ? Color.white : new Color(1f, 1f, 1f, 0.65f);
                }

                _activeIcons.Add(icon);
            }
        }

        private void LoadSprite(Image icon, string spriteKey)
        {
            if (string.IsNullOrEmpty(spriteKey))
            {
                icon.sprite = null;
                return;
            }

#if UNITY_EDITOR
            var sprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>(spriteKey);
            icon.sprite = sprite;
#else
            icon.sprite = null;
#endif
        }

        public void ClearOrder()
        {
            foreach (var icon in _activeIcons)
            {
                icon.sprite = null;
                _iconPool.Release(icon);
            }
            _activeIcons.Clear();
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
