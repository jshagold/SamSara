using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Samsara.Core.Tree;
using Samsara.Features.Character.MasterData;

namespace Samsara.Features.ReplayScene.Domain
{
    public interface IReplayUseCase
    {
        IReadOnlyList<EvolutionNodeSO> GetAllNodes();
        ReplayNodeState ClassifyNode(EvolutionNodeSO node);
        UniTask ExecuteRestartAsync(string selectedNodeId, CancellationToken ct = default);
    }
}
