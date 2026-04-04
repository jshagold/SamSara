using System.Collections.Generic;
using Samsara.Core.MasterData;
using Samsara.Features.Character.MasterData;
using Samsara.Features.Stage.MasterData;

namespace Samsara.Features.Stage.Domain
{
    public interface IStageMasterDataRepository
    {
        void Initialize();
        StageSO GetStageById(string id);
        StageNodeSO GetNodeById(string id);
        IReadOnlyList<StageSO> GetAllStages();
        EnemySO GetEnemyById(int enemyId);
        EvolutionNodeSO GetEvolutionNodeById(string nodeId);
    }
}
