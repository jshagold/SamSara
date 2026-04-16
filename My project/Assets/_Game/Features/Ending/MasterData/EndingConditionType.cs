namespace Samsara.Features.Ending.MasterData
{
    /// <summary>
    /// 엔딩 발동 조건의 종류.
    /// </summary>
    public enum EndingConditionType
    {
        None,             // 조건 없음 — 폴백 EndingSO에 사용
        EvolutionId,      // 현재 진화 ID 일치 (EndingCondition.StringValue 사용)
        EventId,          // 특정 이벤트 ID에서 발동 (EndingCondition.IntValue 사용)
        EventResultType,  // 특정 이벤트 결과 타입에서 발동 (EndingCondition.IntValue — EventResultType을 int로 저장)
        StageCompleteFlag // 끝 노드 여부 (EndingContext.IsStageEndNode == true)
    }
}
