using Cysharp.Threading.Tasks;
using Samsara.Features.Ending.MasterData;

namespace Samsara.Features.Ending.Domain
{
    /// <summary>
    /// 엔딩 진입 공통 서비스 인터페이스.
    /// 트리거 시점 + EndingContext를 받아 매칭 시 EndingScene으로 전환한다.
    /// </summary>
    public interface IEndingEntryService
    {
        /// <summary>
        /// TriggerKind 전역 탐색 경로 (EventResult 등).
        /// 매칭 EndingSO 존재 시 EndingScene 전환 후 true 반환. 매칭 없으면 false.
        /// </summary>
        UniTask<bool> TryEnterEndingAsync(EndingTriggerKind trigger, EndingContext context);

        /// <summary>
        /// BattleNode 슬롯 기반 탐색 경로 (Victory / Defeat).
        /// 슬롯 내 후보만 평가. 매칭 시 EndingScene 전환 후 true. 매칭 없으면 false.
        /// </summary>
        UniTask<bool> TryEnterEndingAsync(EndingCandidateSlot slot, EndingContext context);
    }
}
