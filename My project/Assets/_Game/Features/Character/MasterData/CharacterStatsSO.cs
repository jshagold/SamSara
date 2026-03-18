using UnityEngine;

namespace Samsara.Features.Character.MasterData
{
    [CreateAssetMenu(fileName = "CharacterStatsSO", menuName = "Samsara/Character/CharacterStats")]
    public class CharacterStatsSO : ScriptableObject
    {
        private readonly string _logClass = $"[{nameof(CharacterStatsSO)}]";

        [SerializeField] private int _hp;
        [SerializeField] private int _strength;
        [SerializeField] private int _toughness;
        [SerializeField] private int _speed;

        public int Hp => _hp;
        public int Strength => _strength;
        public int Toughness => _toughness;
        public int Speed => _speed;
    }
}
