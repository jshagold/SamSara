using System.Collections.Generic;
using Samsara.Features.Stage.MasterData;

namespace Samsara.Features.Stage.Domain
{
    public interface IStageMasterDataRepository
    {
        void Initialize();
        StageSO GetStageById(string id);
        StageNodeSO GetNodeById(string id);
        IReadOnlyList<StageSO> GetAllStages();
    }
}
