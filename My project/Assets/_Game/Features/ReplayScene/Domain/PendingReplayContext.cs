using Samsara.Features.Character.MasterData;

namespace Samsara.Features.ReplayScene.Domain
{
    /// <summary>
    /// 씬 간 데이터 전달 — ReplayScene 진입 컨텍스트.
    /// EndingScene 진입 전 설정, ReplayScene 진입 직후 소비. (G-22)
    /// </summary>
    public class PendingReplayContext
    {
        public LastRunResult PreviousRunResult { get; set; }
    }
}
