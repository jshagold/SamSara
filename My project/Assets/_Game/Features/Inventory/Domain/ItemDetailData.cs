using Samsara.Features.Character.MasterData;

namespace Samsara.Features.Inventory.Domain
{
    /// <summary>아이템 길게 누르기 팝업 표시용 상세 데이터.</summary>
    public class ItemDetailData
    {
        public string   ItemName;
        public string   Description;
        public string   IconSpriteKey;
        public StatType TargetStat;
        public int      EffectValue;
        public int      Quantity;
    }
}
