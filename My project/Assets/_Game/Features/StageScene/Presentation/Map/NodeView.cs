using System;
using Samsara.Features.Stage.MasterData;
using UnityEngine;
using UnityEngine.UI;

namespace Samsara.Features.StageScene.Presentation.Map
{
    public enum NodeViewState
    {
        Unvisited,
        Active,
        Current,
        Completed
    }

    public class NodeView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(NodeView)}]";

        [SerializeField] private Image _nodeIcon;
        [SerializeField] private Image _highlightEffect;
        [SerializeField] private Image _completedMark;
        [SerializeField] private Button _button;

        public int NodeIndex { get; private set; }

        public event Action<int> OnNodeClicked;

        private void Awake()
        {
            _button.onClick.AddListener(() => OnNodeClicked?.Invoke(NodeIndex));
        }

        public void Setup(int index, NodeType type, Sprite icon)
        {
            NodeIndex = index;
            _nodeIcon.sprite = icon;
            SetState(NodeViewState.Unvisited);
        }

        public void SetState(NodeViewState state)
        {
            _highlightEffect.gameObject.SetActive(state == NodeViewState.Current);
            _completedMark.gameObject.SetActive(state == NodeViewState.Completed);
            _button.interactable = state == NodeViewState.Active;

            var color = _nodeIcon.color;
            color.a = state == NodeViewState.Unvisited ? 0.5f : 1f;
            _nodeIcon.color = color;
        }

        private void OnDestroy()
        {
            _button?.onClick.RemoveAllListeners();
        }
    }
}
