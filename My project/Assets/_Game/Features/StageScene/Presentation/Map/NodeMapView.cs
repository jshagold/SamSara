using System;
using System.Collections.Generic;
using DG.Tweening;
using Samsara.Features.Stage.MasterData;
using UnityEngine;

namespace Samsara.Features.StageScene.Presentation.Map
{
    public class NodeMapView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(NodeMapView)}]";

        [SerializeField] private NodeView _nodeViewPrefab;
        [SerializeField] private Transform _nodeContainer;
        [SerializeField] private float _nodeSpacing = 150f;
        [SerializeField] private int _viewportRadius = 5;
        [SerializeField] private Sprite[] _nodeTypeIcons;

        private readonly List<NodeView> _nodeViews = new();

        public event Action<int> OnNodeClicked;

        public void RenderNodes(StageNodeSO[] nodes)
        {
            ClearNodes();
            for (var i = 0; i < nodes.Length; i++)
            {
                var nodeView = Instantiate(_nodeViewPrefab, _nodeContainer);
                nodeView.transform.localPosition = new Vector3(0f, i * _nodeSpacing, 0f);

                var icon = GetIconForType(nodes[i].NodeType);
                nodeView.Setup(i, nodes[i].NodeType, icon);
                nodeView.OnNodeClicked += HandleNodeClicked;
                _nodeViews.Add(nodeView);
            }
        }

        public void HighlightNode(int index)
        {
            for (var i = 0; i < _nodeViews.Count; i++)
            {
                if (i == index)
                    _nodeViews[i].SetState(NodeViewState.Current);
                else if (i == index + 1)
                    _nodeViews[i].SetState(NodeViewState.Active);
                // Completed nodes keep their state — only touch Current and next Active
            }
            UpdateViewport(index);
        }

        public void MarkCompleted(int index)
        {
            if (index >= 0 && index < _nodeViews.Count)
                _nodeViews[index].SetState(NodeViewState.Completed);
        }

        public Vector3 GetNodeWorldPosition(int index)
        {
            if (index >= 0 && index < _nodeViews.Count)
                return _nodeViews[index].transform.position;
            return Vector3.zero;
        }

        public void FocusOnNode(int index)
        {
            if (index < 0 || index >= _nodeViews.Count) return;
            var targetY = -(index * _nodeSpacing);
            var currentPos = _nodeContainer.localPosition;
            _nodeContainer.DOLocalMove(new Vector3(currentPos.x, targetY, currentPos.z), 0.4f)
                .SetEase(Ease.OutCubic);
        }

        public void ClearNodes()
        {
            foreach (var nodeView in _nodeViews)
            {
                if (nodeView != null)
                {
                    nodeView.OnNodeClicked -= HandleNodeClicked;
                    Destroy(nodeView.gameObject);
                }
            }
            _nodeViews.Clear();
        }

        private void UpdateViewport(int currentIndex)
        {
            for (var i = 0; i < _nodeViews.Count; i++)
                _nodeViews[i].gameObject.SetActive(Mathf.Abs(i - currentIndex) <= _viewportRadius);
        }

        private Sprite GetIconForType(NodeType type)
        {
            var index = (int)type;
            if (_nodeTypeIcons != null && index < _nodeTypeIcons.Length)
                return _nodeTypeIcons[index];
            return null;
        }

        private void HandleNodeClicked(int index)
        {
            OnNodeClicked?.Invoke(index);
        }
    }
}
