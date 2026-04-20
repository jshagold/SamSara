namespace Samsara.Features.Ending.Domain
{
    public interface IEndingResolver
    {
        /// <summary>
        /// 슬롯의 Candidates를 조건 평가하여 최고 우선순위 EndingSO Id를 반환한다.
        /// 모든 Candidate 매칭 실패 시 Fallback Id를 반환한다.
        /// 슬롯이 비어있거나 Fallback도 없으면 null (런 계속).
        /// </summary>
        int? TryResolve(Samsara.Features.Ending.MasterData.EndingCandidateSlot slot, EndingContext context);
    }
}
