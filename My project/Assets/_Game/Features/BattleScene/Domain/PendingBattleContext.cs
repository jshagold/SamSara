using Samsara.Features.Stage.MasterData;

namespace Samsara.Features.BattleScene.Domain
{
    public class PendingBattleContext
    {
        public BattleNodeDataSO BattleNodeData { get; }

        public PendingBattleContext(BattleNodeDataSO battleNodeData)
        {
            BattleNodeData = battleNodeData;
        }
    }
}
