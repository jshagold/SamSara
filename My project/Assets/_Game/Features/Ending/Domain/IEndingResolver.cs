using Samsara.Features.Ending.MasterData;

namespace Samsara.Features.Ending.Domain
{
    /// <summary>
    /// 현재 플레이어 상태에 따라 EndingType에 해당하는 EndingSO Id를 결정하는 서비스.
    /// </summary>
    public interface IEndingResolver
    {
        /// <summary>
        /// 주어진 EndingType에서 현재 플레이어 상태와 일치하는 EndingSO의 Id를 반환한다.
        /// 일치하는 항목이 없으면 InvalidOperationException (Fail Fast).
        /// </summary>
        int Resolve(EndingType type);
    }
}
