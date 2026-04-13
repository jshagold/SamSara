using Samsara.Features.Character.MasterData;
using UnityEngine;

namespace Samsara.Features.Shop.MasterData
{
    [CreateAssetMenu(fileName = "PotionSO", menuName = "Samsara/Shop/Potion")]
    public class PotionSO : ScriptableObject
    {
        private readonly string _logClass = $"[{nameof(PotionSO)}]";

        [SerializeField] private int     _id;
        [SerializeField] private string  _potionName;
        [SerializeField] private string  _description;
        [SerializeField] private StatType _targetStat;
        [SerializeField] private int     _effectValue;
        [SerializeField] private int     _price;
        [SerializeField] private Sprite  _sprite;

        public int      Id          => _id;
        public string   PotionName  => _potionName;
        public string   Description => _description;
        public StatType TargetStat  => _targetStat;
        public int      EffectValue => _effectValue;
        public int      Price       => _price;
        public Sprite   Sprite      => _sprite;
    }
}
