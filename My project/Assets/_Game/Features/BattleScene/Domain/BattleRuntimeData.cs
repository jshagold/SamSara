using System.Collections.Generic;

namespace Samsara.Features.BattleScene.Domain
{
    public enum BattlePhase
    {
        SkillSelect,
        QTE,
        DamageProcess,
        Result
    }

    /// <summary>
    /// 전투 전체 런타임 상태 (DTO). 로직 없음.
    /// BattleUseCase가 소유하고 직접 변경한다.
    /// </summary>
    public class BattleRuntimeData
    {
        private List<BattleParticipant> _allies;
        private List<BattleParticipant> _enemies;
        private int _turnNumber;
        private BattlePhase _currentPhase;

        public List<BattleParticipant> Allies        { get => _allies;        set => _allies = value; }
        public List<BattleParticipant> Enemies       { get => _enemies;       set => _enemies = value; }
        public int TurnNumber                        { get => _turnNumber;    set => _turnNumber = value; }
        public BattlePhase CurrentPhase              { get => _currentPhase;  set => _currentPhase = value; }
    }
}
