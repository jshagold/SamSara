using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Samsara.Features.Stage.Data;

namespace Samsara.Features.Stage.Domain
{
    public interface IStageRepository
    {
        StageRunData RunData { get; }
        void InitializeRun(string startStageId);
        void CompleteNode(int nodeIndex);
        void TransitionToStage(string stageId);
        void SetGeneratedNodes(List<string> nodeIds);
        UniTask SaveAsync();
        void SaveSync();
        UniTask LoadAsync();
    }
}
