using Samsara.Features.Stage.MasterData;

namespace Samsara.Features.Stage.Domain
{
    /// <summary>
    /// StageProgressService.CompleteNodeAsync에 전달하는 노드 완료 컨텍스트.
    /// </summary>
    public struct NodeCompletionContext
    {
        /// <summary>완료된 노드 인덱스.</summary>
        public int      NodeIndex;
        /// <summary>노드 타입 (Battle / Event / etc.).</summary>
        public NodeType NodeType;
        /// <summary>현재 노드가 스테이지 끝 노드인지 여부.</summary>
        public bool     IsStageEndNode;
    }
}
