using System.Collections.Generic;

namespace Samsara.Features.Stage.Data
{
    public class StageRunData
    {
        public string CurrentStageId;
        public int CurrentNodeIndex;
        public List<string> GeneratedNodeIds = new();
        public List<int> CompletedNodeIndices = new();
    }
}
