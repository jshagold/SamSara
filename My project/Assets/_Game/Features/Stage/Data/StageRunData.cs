using System.Collections.Generic;

namespace Samsara.Features.Stage.Data
{
    public class StageRunData
    {
        public string CurrentStageId;
        public int CurrentNodeIndex;
        public List<string> GeneratedNodeIds = new();
        public List<int> CompletedNodeIndices = new();
        public int PendingChainedEventId = -1;
        /// <summary>현재 런에서 클리어한 스테이지 수. 보스 전투 승리 시 증가.</summary>
        public int ClearedStageCount;
    }
}
