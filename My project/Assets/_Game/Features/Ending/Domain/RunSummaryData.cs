namespace Samsara.Features.Ending.Domain
{
    /// <summary>
    /// 엔딩 결과 팝업에 표시할 런 요약 데이터.
    /// </summary>
    public class RunSummaryData
    {
        public int    TotalDays          { get; set; }
        public string FinalEvolutionName { get; set; }
        public int    StagesCleared      { get; set; }
        public int    FinalGold          { get; set; }
    }
}
