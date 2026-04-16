namespace Samsara.Features.Ending.MasterData
{
    /// <summary>
    /// 엔딩 발동 조건의 종류.
    /// </summary>
    public enum EndingConditionType
    {
        None,        // 조건 없음 — 폴백 EndingSO에 사용
        EvolutionId  // 현재 진화 ID 일치 (EndingCondition.StringValue 사용)

        // 미래 확장 (이번 Patch 미구현, 구조만 준비):
        // StatRange, KarmaRange, UnlockedEventId, HpRatio 등
    }
}
