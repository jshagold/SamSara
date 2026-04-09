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
            Canvas.ForceUpdateCanvases();
            var viewportSize = _scrollRect.viewport.rect.size;

            _content.sizeDelta = new Vector2(layoutResult.ContentWidth, layoutResult.ContentHeight);

            // Shift nodes down by half viewport to create top padding (pivot is top-center)
            float verticalPadding = viewportSize.y / 2f;

            Debug.Log($"{_logClass} Content sizeDelta={_content.sizeDelta} viewport={viewportSize} verticalPadding={verticalPadding}");

            foreach (var nodeLayout in layoutResult.Nodes)
            {
                var nodeView = Instantiate(_nodePrefab, _content);
                var rt = nodeView.GetComponent<RectTransform>();
                rt.anchorMin = new Vector2(0.5f, 1f);
                rt.anchorMax = new Vector2(0.5f, 1f);
                rt.pivot = new Vector2(0.5f, 0.5f);
                rt.anchoredPosition = new Vector2(nodeLayout.X, nodeLayout.Y - verticalPadding);

                Debug.Log($"{_logClass} Node [{nodeLayout.NodeId}] anchoredPosition={rt.anchoredPosition}");

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
            var viewportSize = _scrollRect.viewport.rect.size;
            // Content uses stretch anchors (0,0)-(1,1), so actual size = viewport + sizeDelta
            var contentSize = viewportSize + _content.sizeDelta;

            float scrollableX = contentSize.x - viewportSize.x;
            float scrollableY = contentSize.y - viewportSize.y;

            // Node distance from content edges (content pivot = 0.5, 1 = top-center)
            float nodeDistFromLeft = contentSize.x / 2f + nodeRt.anchoredPosition.x;
            float nodeDistFromTop = Mathf.Abs(nodeRt.anchoredPosition.y);

            // Center node in viewport
            float normalizedX = scrollableX > 0f
                ? Mathf.Clamp01((nodeDistFromLeft - viewportSize.x / 2f) / scrollableX)
                : 0.5f;
            float normalizedY = scrollableY > 0f
                ? Mathf.Clamp01(1f - (nodeDistFromTop - viewportSize.y / 2f) / scrollableY)
                : 0.5f;

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
