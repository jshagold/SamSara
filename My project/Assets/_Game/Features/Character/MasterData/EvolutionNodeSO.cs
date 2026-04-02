using System;
using UnityEngine;

namespace Samsara.Features.Character.MasterData
{
    [CreateAssetMenu(fileName = "EvolutionNodeSO", menuName = "Samsara/Character/EvolutionNode")]
    public class EvolutionNodeSO : ScriptableObject
    {
        private readonly string _logClass = $"[{nameof(EvolutionNodeSO)}]";

        [SerializeField] private string _nodeId;
        [SerializeField] private string _characterName;
        [SerializeField] private CharacterStatsSO _baseStats;
        [SerializeField] private int _maxActionPoints;
        [SerializeField] private StatCondition[] _unlockConditions;
        [SerializeField] private bool _isHidden;
        [SerializeField] private EvolutionNodeSO[] _nextNodes;

        // Sprite fields (Addressables keys)
        [SerializeField] private string _nodeIconSpriteKey;
        [SerializeField] private string _mainStandingSpriteKey;
        [SerializeField] private string _portraitSpriteKey;
        [SerializeField] private string _battleSpriteKeyHp100;
        [SerializeField] private string _battleSpriteKeyHp50;
        [SerializeField] private string _battleSpriteKeyHp0;
        [SerializeField] private string _attackAnimSpriteKey;
        [SerializeField] private string _stageMoveSpriteKey;
        [SerializeField] private int[] _skillIds;

        public string NodeId => _nodeId;
        public string CharacterName => _characterName;
        public CharacterStatsSO BaseStats => _baseStats;
        public int MaxActionPoints => _maxActionPoints;
        public StatCondition[] UnlockConditions => _unlockConditions;
        public bool IsHidden => _isHidden;
        public EvolutionNodeSO[] NextNodes => _nextNodes;
        public string NodeIconSpriteKey => _nodeIconSpriteKey;
        public string MainStandingSpriteKey => _mainStandingSpriteKey;
        public string PortraitSpriteKey => _portraitSpriteKey;
        public string BattleSpriteKeyHp100 => _battleSpriteKeyHp100;
        public string BattleSpriteKeyHp50 => _battleSpriteKeyHp50;
        public string BattleSpriteKeyHp0 => _battleSpriteKeyHp0;
        public string AttackAnimSpriteKey => _attackAnimSpriteKey;
        public string StageMoveSpriteKey => _stageMoveSpriteKey;
        public int[] SkillIds => _skillIds;
    }

    [Serializable]
    public class StatCondition
    {
        [SerializeField] private StatType _statType;
        [SerializeField] private int _requiredValue;

        public StatType StatType => _statType;
        public int RequiredValue => _requiredValue;
    }
}
