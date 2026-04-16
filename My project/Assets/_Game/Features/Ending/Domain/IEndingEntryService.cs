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
        /// 매칭 EndingSO 존재 시 EndingScene 전환 후 true 반환.
        /// 매칭 없으면 false 반환 (호출자가 런 계속 처리).
        /// </summary>
        UniTask<bool> TryEnterEndingAsync(EndingTriggerKind trigger, EndingContext context);
    }
}
