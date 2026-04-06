using System.Collections.Generic;
using Samsara.Features.EvolutionTreeScene.Domain;
using UnityEngine;
using UnityEngine.UI;

namespace Samsara.Features.EvolutionTreeScene.Presentation.TreeArea
{
    public class TreeScrollView : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(TreeScrollView)}]";

        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private RectTransform _content;
        [SerializeField] private EvolutionNodeView _nodePrefab;
        [SerializeField] private NodeConnectionView _connectionPrefab;

        private readonly List<EvolutionNodeView> _nodeViews = new List<EvolutionNodeView>();
        private readonly Dictionary<string, EvolutionNodeView> _nodeViewMap = new Dictionary<string, EvolutionNodeView>();

        public void BuildTree(TreeLayoutResult layoutResult)
        {
            _content.sizeDelta = new Vector2(layoutResult.ContentWidth, layoutResult.ContentHeight);

            // Offset so center-aligned coords map to content space
            float offsetX = layoutResult.ContentWidth / 2f;
            float offsetY = -layoutResult.ContentHeight / 2f;

            foreach (var nodeLayout in layoutResult.Nodes)
            {
                var nodeView = Instantiate(_nodePrefab, _content);
                var rt = nodeView.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(0.5f, 1f);
                rt.anchorMax = new Vector2(0.5f, 1f);
                rt.pivot = new Vector2(0.5f, 0.5f);
                rt.anchoredPosition = new Vector2(nodeLayout.X, nodeLayout.Y);

                _nodeViews.Add(nodeView);
                _nodeViewMap[nodeLayout.NodeId] = nodeView;
            }

            foreach (var conn in layoutResult.Connections)
            {
                if (!_nodeViewMap.ContainsKey(conn.ParentNodeId) ||
                    !_nodeViewMap.ContainsKey(conn.ChildNodeId)) continue;

                var parentRt = _nodeViewMap[conn.ParentNodeId].GetComponent<RectTransform>();
                var childRt = _nodeViewMap[conn.ChildNodeId].GetComponent<RectTransform>();

                var connectionView = Instantiate(_connectionPrefab, _content);
                connectionView.transform.SetAsFirstSibling();
                connectionView.SetConnection(parentRt.anchoredPosition, childRt.anchoredPosition);
            }
        }

        public EvolutionNodeView GetNodeView(string nodeId)
        {
            _nodeViewMap.TryGetValue(nodeId, out var view);
            return view;
        }

        public void ScrollToNode(string nodeId)
        {
            if (!_nodeViewMap.TryGetValue(nodeId, out var nodeView)) return;

            var nodeRt = nodeView.GetComponent<RectTransform>();
            var contentSize = _content.sizeDelta;
            var viewportSize = _scrollRect.viewport.rect.size;

            float normalizedX = Mathf.Clamp01(
                (nodeRt.anchoredPosition.x + contentSize.x / 2f) / (contentSize.x - viewportSize.x));
            float normalizedY = Mathf.Clamp01(
                1f - (-nodeRt.anchoredPosition.y) / (contentSize.y - viewportSize.y));

            _scrollRect.normalizedPosition = new Vector2(normalizedX, normalizedY);
        }

        public EvolutionNodeView[] GetAllNodeViews()
        {
            return _nodeViews.ToArray();
        }

        private void Reset()
        {
            _scrollRect = GetComponentInChildren<ScrollRect>();
        }
    }
}
