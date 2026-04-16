using Cysharp.Threading.Tasks;
using Samsara.Features.Ending.MasterData;

namespace Samsara.Features.Ending.Domain
{
    /// <summary>
    /// 엔딩 진입 공통 서비스 인터페이스.
    /// BattleScene, EventScene 등 런이 종료되는 모든 씬에서 사용한다.
    /// </summary>
    public interface IEndingEntryService
    {
        /// <summary>
        /// 지정된 EndingType에 해당하는 엔딩을 결정하고 EndingScene으로 전환한다.
        /// </summary>
        UniTask EnterEndingAsync(EndingType endingType);
    }
}
