public enum EvolutionStateType
{
    None = 0,

    Locked,         // 선행 조건 미달성
    Impossible,     // 선행 달성, 조건(스탯) 미달
    Possible,       // 진화 가능 (스탯 충족)
    Current,        // 현재 단계
    Completed       // 이미 지나간 단계
}