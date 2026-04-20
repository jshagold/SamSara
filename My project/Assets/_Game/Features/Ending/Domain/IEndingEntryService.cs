using Cysharp.Threading.Tasks;
using Samsara.Features.Ending.MasterData;

namespace Samsara.Features.Ending.Domain
{
    /// <summary>
    /// 엔딩 진입 공통 서비스 인터페이스.
    /// EndingCandidateSlot + EndingContext를 받아 매칭 시 EndingScene으로 전환한다.
    /// </summary>
    public interface IEndingEntryService
    {
        /// <summary>
        /// 슬롯 기반 탐색. 매칭(또는 Fallback) 시 EndingScene 전환 후 true. 매칭 없으면 false.
        /// </summary>
        UniTask<bool> TryEnterEndingAsync(EndingCandidateSlot slot, EndingContext context);
    }
}
