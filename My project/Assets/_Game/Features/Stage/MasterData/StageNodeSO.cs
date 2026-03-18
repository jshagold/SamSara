using UnityEngine;

namespace Samsara.Features.Stage.MasterData
{
    [CreateAssetMenu(fileName = "StageNodeSO", menuName = "Samsara/Stage/StageNode")]
    public class StageNodeSO : ScriptableObject
    {
        private readonly string _logClass = $"[{nameof(StageNodeSO)}]";

        [SerializeField] private string _nodeId;
        [SerializeField] private NodeType _nodeType;
        [SerializeField] private string _nodeSpriteKey;
        [SerializeField] private BattleNodeDataSO _battleData;
        [SerializeField] private EventNodeDataSO _eventData;

        public string NodeId => _nodeId;
        public NodeType NodeType => _nodeType;
        public string NodeSpriteKey => _nodeSpriteKey;
        public BattleNodeDataSO BattleData => _battleData;
        public EventNodeDataSO EventData => _eventData;
    }
}
