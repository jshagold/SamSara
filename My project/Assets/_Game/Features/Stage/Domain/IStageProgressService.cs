using Cysharp.Threading.Tasks;

namespace Samsara.Features.Stage.Domain
{
    /// <summary>
    /// 노드 완료 처리 공통 서비스 인터페이스.
    /// 끝 노드 여부는 서비스 내부에서 판단 — 호출자(DTO)가 IsStageEndNode를 전달하지 않는다.
    /// 끝 노드 → ClearedStageCount++ + 저장.
    /// 일반 노드 → Day++, AP 리셋, CurrentNodeIndex++ + 저장.
    /// </summary>
    public interface IStageProgressService
    {
        UniTask CompleteNodeAsync(NodeCompletionContext context);
    }
}
