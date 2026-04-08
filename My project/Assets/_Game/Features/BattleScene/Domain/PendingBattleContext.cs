using Samsara.Features.Stage.MasterData;

namespace Samsara.Features.BattleScene.Domain
{
    public class PendingBattleContext
    {
        public BattleNodeDataSO BattleNodeData { get; }
        public BattleEventData[] BattleEvents { get; }  // nullable — null means no event hooks

        public PendingBattleContext(BattleNodeDataSO battleNodeData, BattleEventData[] battleEvents = null)
        {
            BattleNodeData = battleNodeData;
            BattleEvents = battleEvents;
        }
    }
}
