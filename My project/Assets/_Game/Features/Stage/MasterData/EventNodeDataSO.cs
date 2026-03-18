using UnityEngine;

namespace Samsara.Features.Stage.MasterData
{
    [CreateAssetMenu(fileName = "EventNodeDataSO", menuName = "Samsara/Stage/EventNodeData")]
    public class EventNodeDataSO : ScriptableObject
    {
        private readonly string _logClass = $"[{nameof(EventNodeDataSO)}]";

        [SerializeField] private int _eventId;

        public int EventId => _eventId;
    }
}
