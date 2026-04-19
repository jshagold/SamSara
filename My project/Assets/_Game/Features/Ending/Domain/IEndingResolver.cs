using Samsara.Features.Ending.MasterData;

namespace Samsara.Features.Ending.Domain
{
    /// <summary>
    /// 트리거 시점 + EndingContext 기반으로 매칭 EndingSO Id를 결정하는 서비스.
    /// </summary>
    public interface IEndingResolver
    {
        /// <summary>
        /// TriggerKind 전역 필터 + 조건 평가로 최고 우선순위 Id를 반환한다.
        /// 매칭 없으면 null (EventResult 트리거 등 전역 탐색 경로).
        /// </summary>
        int? TryResolve(EndingTriggerKind trigger, EndingContext context);

        /// <summary>
        /// 슬롯 내 후보 배열만 평가해 최고 우선순위 Id를 반환한다.
        /// 매칭 없으면 null (BattleNode 슬롯 기반 경로).
        /// </summary>
        int? TryResolve(EndingSO[] candidates, EndingContext context);
    }
}
