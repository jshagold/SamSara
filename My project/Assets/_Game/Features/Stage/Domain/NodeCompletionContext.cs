using Samsara.Features.Stage.MasterData;

namespace Samsara.Features.Stage.Domain
{
    /// <summary>
    /// StageProgressService.CompleteNodeAsync에 전달하는 노드 완료 컨텍스트.
    /// 순수 DTO — 끝 노드 판단은 StageProgressService 내부에서 수행한다.
    /// </summary>
    public struct NodeCompletionContext
    {
        /// <summary>완료된 노드 인덱스.</summary>
        public int      NodeIndex;
        /// <summary>노드 타입 (Battle / Event / etc.).</summary>
        public NodeType NodeType;
    }
}
