namespace Samsara.Features.Ending.MasterData
{
    /// <summary>
    /// 엔딩 발동 시점 종류. EndingSO._triggerKind 에 설정한다.
    /// </summary>
    public enum EndingTriggerKind
    {
        BattleVictory,   // 전투 승리 시점
        BattleDefeat,    // 전투 패배 시점
        EventResult      // 이벤트 결과 시점

        // 미래 확장: SpecialCondition (N턴 생존 등)
    }
}
