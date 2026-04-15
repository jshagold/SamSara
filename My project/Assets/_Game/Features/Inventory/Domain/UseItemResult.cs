using Samsara.Features.Character.MasterData;

namespace Samsara.Features.Inventory.Domain
{
    public enum UseItemFailReason
    {
        None,
        EmptySlot,
        NotUsable
    }

    public class UseItemResult
    {
        public bool             IsSuccess    { get; private set; }
        public UseItemFailReason FailReason  { get; private set; }

        /// <summary>성공 시에만 유효. 영향받은 스탯.</summary>
        public StatType AffectedStat { get; private set; }

        /// <summary>성공 시에만 유효. 적용된 효과량.</summary>
        public int EffectValue { get; private set; }

        public static UseItemResult Success(StatType stat, int value) => new UseItemResult
        {
            IsSuccess    = true,
            FailReason   = UseItemFailReason.None,
            AffectedStat = stat,
            EffectValue  = value
        };

        public static UseItemResult Fail(UseItemFailReason reason) => new UseItemResult
        {
            IsSuccess  = false,
            FailReason = reason
        };
    }
}
