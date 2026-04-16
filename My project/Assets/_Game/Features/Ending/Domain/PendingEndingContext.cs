namespace Samsara.Features.Ending.Domain
{
    /// <summary>
    /// 씬 간 데이터 전달 — EndingScene 진입 컨텍스트.
    /// EndingScene 진입 전 설정, 진입 후 즉시 소비.
    /// </summary>
    public class PendingEndingContext
    {
        public int            EndingId   { get; set; }
        public RunSummaryData RunSummary { get; set; }
    }
}
