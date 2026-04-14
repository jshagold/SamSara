using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Samsara.Features.Stage.Data;

namespace Samsara.Features.Stage.Domain
{
    public interface IStageRepository
    {
        StageRunData RunData { get; }
        void InitializeNewRun(RunConfigSO config);
        void CompleteNode(int nodeIndex);
        void TransitionToStage(string stageId);
        void SetGeneratedNodes(List<string> nodeIds);
        void SetPendingChainedEventId(int eventId);
        UniTask SaveAsync();
        void SaveSync();
        UniTask LoadAsync();
    }
}
