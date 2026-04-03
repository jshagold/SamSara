using System.Collections.Generic;

namespace Samsara.Features.BattleScene.Domain
{
    /// <summary>
    /// 전투 참여자 런타임 데이터 (DTO). 아군/적군 공용.
    /// BattleUseCase가 직접 값을 수정한다.
    /// </summary>
    public class BattleParticipant
    {
        private int _id;
        private bool _isAlly;
        private int _currentHp;
        private int _maxHp;
        private int _strength;
        private int _toughness;
        private int _agility;
        private float _actionGauge;
        private int[] _skillIds;
        private Dictionary<int, int> _skillCooldowns;
        private bool _isDead;
        private string _spriteKey;

        public int Id                               { get => _id;             set => _id = value; }
        public bool IsAlly                          { get => _isAlly;         set => _isAlly = value; }
        public int CurrentHp                        { get => _currentHp;      set => _currentHp = value; }
        public int MaxHp                            { get => _maxHp;          set => _maxHp = value; }
        public int Strength                         { get => _strength;       set => _strength = value; }
        public int Toughness                        { get => _toughness;      set => _toughness = value; }
        public int Agility                          { get => _agility;        set => _agility = value; }
        public float ActionGauge                    { get => _actionGauge;    set => _actionGauge = value; }
        public int[] SkillIds                       { get => _skillIds;       set => _skillIds = value; }
        public Dictionary<int, int> SkillCooldowns  { get => _skillCooldowns; set => _skillCooldowns = value; }
        public bool IsDead                          { get => _isDead;         set => _isDead = value; }
        public string SpriteKey                     { get => _spriteKey;      set => _spriteKey = value; }
    }
}
