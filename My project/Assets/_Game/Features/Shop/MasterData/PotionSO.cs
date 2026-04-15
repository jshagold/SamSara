using Samsara.Features.Character.MasterData;
using Samsara.Features.Inventory.Domain;
using UnityEngine;

namespace Samsara.Features.Shop.MasterData
{
    [CreateAssetMenu(fileName = "PotionSO", menuName = "Samsara/Shop/Potion")]
    public class PotionSO : ScriptableObject, IItemData
    {
        private readonly string _logClass = $"[{nameof(PotionSO)}]";

        [SerializeField] private int      _id;
        [SerializeField] private string   _potionName;
        [SerializeField] private string   _description;
        [SerializeField] private StatType _targetStat;
        [SerializeField] private int      _effectValue;
        [SerializeField] private int      _price;
        [SerializeField] private string   _spriteKey;

        // PotionSO 고유 프로퍼티
        public int      Id          => _id;
        public string   PotionName  => _potionName;
        public string   Description => _description;
        public StatType TargetStat  => _targetStat;
        public int      EffectValue => _effectValue;
        public int      Price       => _price;
        public string   SpriteKey   => _spriteKey;

        // IItemData 구현
        string   IItemData.ItemName      => _potionName;
        ItemType IItemData.ItemType      => ItemType.Consumable;
        string   IItemData.IconSpriteKey => _spriteKey;
    }
}
