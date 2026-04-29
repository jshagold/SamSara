using System;
using UnityEngine;
using UnityEngine.UI;

namespace Samsara.Core.Tree
{
    public class NodeView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(NodeView)}]";

        [SerializeField] private Button _button;
        [SerializeField] private Image _nodeIcon;
        [SerializeField] private Image _frameImage;

        private string _nodeId;
        public string NodeId => _nodeId;

        public event Action<string> OnNodeClicked;

        public void Setup(string nodeId, Sprite icon)
        {
            _nodeId = nodeId;
            SetIconSprite(icon);
        }

        public void SetFrameSprite(Sprite frameSprite)
        {
            _frameImage.sprite = frameSprite;
        }

        public void SetIconSprite(Sprite iconSprite)
        {
            _nodeIcon.sprite = iconSprite;
            _nodeIcon.enabled = iconSprite != null;
        }

        private void OnEnable()
        {
            _button.onClick.AddListener(HandleNodeClicked);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(HandleNodeClicked);
        }

        private void HandleNodeClicked()
        {
            OnNodeClicked?.Invoke(_nodeId);
        }

        private void Reset()
        {
            _button = GetComponentInChildren<Button>();
            _nodeIcon = GetComponentInChildren<Image>();
        }
    }
}
