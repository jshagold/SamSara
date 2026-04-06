using System;
using System.Collections.Generic;
using Samsara.Features.Character.MasterData;
using UnityEngine;

namespace Samsara.Features.EvolutionTreeScene.Domain
{
    public struct NodeLayoutData
    {
        public string NodeId;
        public float X;
        public float Y;
        public int Depth;
    }

    public struct ConnectionLayoutData
    {
        public string ParentNodeId;
        public string ChildNodeId;
    }

    public struct TreeLayoutResult
    {
        public NodeLayoutData[] Nodes;
        public ConnectionLayoutData[] Connections;
        public float ContentWidth;
        public float ContentHeight;
    }

    public class TreeLayoutCalculator
    {
        private readonly string _logClass = $"[{nameof(TreeLayoutCalculator)}]";

        private const float NodeSpacingX = 200f;
        private const float NodeSpacingY = 250f;

        public TreeLayoutResult CalculateLayout(EvolutionNodeSO[] allNodes)
        {
            var root = FindRoot(allNodes);
            if (root == null)
                throw new InvalidOperationException($"{_logClass} Root node not found.");

            // BFS depth calculation
            var depthMap = new Dictionary<string, int>();
            var queue = new Queue<(EvolutionNodeSO node, int depth)>();
            queue.Enqueue((root, 0));
            depthMap[root.NodeId] = 0;

            var connections = new List<ConnectionLayoutData>();

            while (queue.Count > 0)
            {
                var (current, currentDepth) = queue.Dequeue();
                if (current.NextNodes == null) continue;

                foreach (var child in current.NextNodes)
                {
                    if (child == null) continue;
                    if (depthMap.ContainsKey(child.NodeId)) continue;

                    depthMap[child.NodeId] = currentDepth + 1;
                    queue.Enqueue((child, currentDepth + 1));

                    connections.Add(new ConnectionLayoutData
                    {
                        ParentNodeId = current.NodeId,
                        ChildNodeId = child.NodeId
                    });
                }
            }

            // Group nodes by depth
            int maxDepth = 0;
            var depthGroups = new Dictionary<int, List<string>>();
            foreach (var kvp in depthMap)
            {
                if (!depthGroups.ContainsKey(kvp.Value))
                    depthGroups[kvp.Value] = new List<string>();
                depthGroups[kvp.Value].Add(kvp.Key);
                if (kvp.Value > maxDepth) maxDepth = kvp.Value;
            }

            // Calculate per-depth horizontal center alignment
            var nodeLayouts = new List<NodeLayoutData>();
            float globalMaxWidth = 0f;

            for (int depth = 0; depth <= maxDepth; depth++)
            {
                if (!depthGroups.ContainsKey(depth)) continue;

                var nodesAtDepth = depthGroups[depth];
                int count = nodesAtDepth.Count;
                float totalWidth = (count - 1) * NodeSpacingX;
                float startX = -totalWidth / 2f;

                if (totalWidth > globalMaxWidth) globalMaxWidth = totalWidth;

                for (int i = 0; i < count; i++)
                {
                    nodeLayouts.Add(new NodeLayoutData
                    {
                        NodeId = nodesAtDepth[i],
                        X = startX + i * NodeSpacingX,
                        Y = -depth * NodeSpacingY,
                        Depth = depth
                    });
                }
            }

            float contentWidth = globalMaxWidth + NodeSpacingX;
            float contentHeight = maxDepth * NodeSpacingY + NodeSpacingY;

            return new TreeLayoutResult
            {
                Nodes = nodeLayouts.ToArray(),
                Connections = connections.ToArray(),
                ContentWidth = contentWidth,
                ContentHeight = contentHeight
            };
        }

        private EvolutionNodeSO FindRoot(EvolutionNodeSO[] allNodes)
        {
            // Root = node not referenced in any other node's NextNodes
            var referencedIds = new HashSet<string>();
            foreach (var node in allNodes)
            {
                if (node.NextNodes == null) continue;
                foreach (var child in node.NextNodes)
                {
                    if (child != null)
                        referencedIds.Add(child.NodeId);
                }
            }

            foreach (var node in allNodes)
            {
                if (!referencedIds.Contains(node.NodeId))
                    return node;
            }

            return null;
        }
    }
}
