using UnityEngine;
using Samsara.Features.Character.MasterData;

namespace Samsara.Core.MasterData
{
    [CreateAssetMenu(fileName = "EnemySO", menuName = "Samsara/Core/Enemy")]
    public class EnemySO : ScriptableObject
    {
        private readonly string _logClass = $"[{nameof(EnemySO)}]";

        [SerializeField] private int _enemyId;
        [SerializeField] private string _enemyName;
        [SerializeField] private CharacterStatsSO _baseStats;
        [SerializeField] private int[] _skillIds;  // SkillSO ID references

        // Sprite fields (Addressables keys)
        [SerializeField] private string _battleSpriteKeyHp100;
        [SerializeField] private string _battleSpriteKeyHp50;
        [SerializeField] private string _battleSpriteKeyHp0;
        [SerializeField] private string _portraitSpriteKey;

        public int EnemyId => _enemyId;
        public string EnemyName => _enemyName;
        public CharacterStatsSO BaseStats => _baseStats;
        public int[] SkillIds => _skillIds;
        public string BattleSpriteKeyHp100 => _battleSpriteKeyHp100;
        public string BattleSpriteKeyHp50 => _battleSpriteKeyHp50;
        public string BattleSpriteKeyHp0 => _battleSpriteKeyHp0;
        public string PortraitSpriteKey => _portraitSpriteKey;
    }
}
