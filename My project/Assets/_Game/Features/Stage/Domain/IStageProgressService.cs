using Cysharp.Threading.Tasks;

namespace Samsara.Features.Stage.Domain
{
    /// <summary>
    /// 노드 완료 처리 공통 서비스 인터페이스.
    /// 끝 노드 여부에 따라 ClearedStageCount 증가 또는 MoveToNode를 처리한다.
    /// </summary>
    public interface IStageProgressService
    {
        /// <summary>
        /// 노드 완료 처리.
        /// IsStageEndNode=true  → ClearedStageCount++ + 저장 (스테이지 클리어 데이터 확정).
        /// IsStageEndNode=false → Day++, AP 리셋, CurrentNodeIndex++ + 저장.
        /// </summary>
        UniTask CompleteNodeAsync(NodeCompletionContext context);
    }
}
