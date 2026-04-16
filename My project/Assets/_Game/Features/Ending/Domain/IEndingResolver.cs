using Samsara.Features.Ending.MasterData;

namespace Samsara.Features.Ending.Domain
{
    /// <summary>
    /// 트리거 시점 + EndingContext 기반으로 매칭 EndingSO Id를 결정하는 서비스.
    /// </summary>
    public interface IEndingResolver
    {
        /// <summary>
        /// 주어진 TriggerKind + EndingContext에서 조건을 만족하는 최고 우선순위 EndingSO Id를 반환한다.
        /// 매칭 없으면 null 반환 (런 계속 진행).
        /// </summary>
        int? TryResolve(EndingTriggerKind trigger, EndingContext context);
    }
}
