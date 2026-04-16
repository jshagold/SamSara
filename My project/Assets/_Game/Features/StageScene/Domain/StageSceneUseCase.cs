using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Samsara.Features.Character.Domain;
using Samsara.Features.Stage.Domain;
using Samsara.Features.Stage.MasterData;

namespace Samsara.Features.StageScene.Domain
{
    public class StageSceneViewModel
    {
        public string CurrentStageId;
        public int CurrentNodeIndex;
        public List<int> CompletedNodeIndices;
        public int Day;
        public string CurrentEvolutionNodeId;
        public string BiomeSpriteKey;
        public bool CanReturnToMain;
    }

    public class StageSceneUseCase
    {
        private readonly string _logClass = $"[{nameof(StageSceneUseCase)}]";

        private readonly IStageRepository _stageRepo;
        private readonly IStageMasterDataRepository _stageMasterDataRepo;
        private readonly ICharacterRunRepository _characterRunRepo;

        public StageSceneUseCase(
            IStageRepository stageRepo,
            IStageMasterDataRepository stageMasterDataRepo,
            ICharacterRunRepository characterRunRepo)
        {
            _stageRepo = stageRepo;
            _stageMasterDataRepo = stageMasterDataRepo;
            _characterRunRepo = characterRunRepo;
        }

        public StageSceneViewModel GetStageSceneViewModel()
        {
            var stageData = _stageRepo.RunData;
            var characterData = _characterRunRepo.RunData;
            return new StageSceneViewModel
            {
                CurrentStageId       = stageData.CurrentStageId,
                CurrentNodeIndex     = stageData.CurrentNodeIndex,
                CompletedNodeIndices = stageData.CompletedNodeIndices,
                Day                  = characterData.Day,
                CurrentEvolutionNodeId = characterData.EvolutionNodeId,
                BiomeSpriteKey       = GetBiomeBackground(),
                CanReturnToMain      = CanReturnToMain()
            };
        }

        public StageNodeSO[] GetCurrentStageNodes()
        {
            var stageData = _stageRepo.RunData;
            var stageSO = _stageMasterDataRepo.GetStageById(stageData.CurrentStageId);

            if (stageSO.IsFixed)
                return stageSO.FixedNodes;

            var nodeIds = stageData.GeneratedNodeIds;
            var nodes = new StageNodeSO[nodeIds.Count];
            for (var i = 0; i < nodeIds.Count; i++)
                nodes[i] = _stageMasterDataRepo.GetNodeById(nodeIds[i]);
            return nodes;
        }

        public async UniTask MoveToNode(int targetIndex)
        {
            _characterRunRepo.RunData.Day += 1;
            _characterRunRepo.RunData.ActionPoints = _characterRunRepo.RunData.MaxActionPoints;
            _stageRepo.CompleteNode(_stageRepo.RunData.CurrentNodeIndex);
            _characterRunRepo.MarkDirty();
            await UniTask.WhenAll(
                _stageRepo.SaveAsync(),
                _characterRunRepo.SaveDataAsync()
            );
        }

        public NodeType GetNodeType(int index)
        {
            var nodes = GetCurrentStageNodes();
            return nodes[index].NodeType;
        }

        public bool CanReturnToMain()
        {
            var nodes = GetCurrentStageNodes();
            var currentIndex = _stageRepo.RunData.CurrentNodeIndex;
            if (currentIndex < 0 || currentIndex >= nodes.Length) return true;
            return nodes[currentIndex].CanReturnToMain;
        }

        /// <summary>
        /// 해당 nodeIndex가 스테이지 끝 노드인지 여부를 반환한다.
        /// 끝 노드 = 노드 배열의 마지막 인덱스 (nodeIndex >= nodes.Length - 1).
        /// </summary>
        public bool IsStageComplete(int nodeIndex)
        {
            var nodes = GetCurrentStageNodes();
            if (nodes.Length == 0 || nodeIndex < 0) return false;
            return nodeIndex >= nodes.Length - 1;
        }

        public List<StageSO> GetNextStageOptions()
        {
            var stageSO = _stageMasterDataRepo.GetStageById(_stageRepo.RunData.CurrentStageId);
            var result = new List<StageSO>();
            foreach (var nextId in stageSO.NextStageIds)
                result.Add(_stageMasterDataRepo.GetStageById(nextId));
            return result;
        }

        public async UniTask SelectNextStage(string stageId)
        {
            _stageRepo.TransitionToStage(stageId);

            var newStageSO = _stageMasterDataRepo.GetStageById(stageId);
            if (!newStageSO.IsFixed)
            {
                // TODO: [BACKLOG] Random node generation algorithm not specified in spec.
                _stageRepo.SetGeneratedNodes(new List<string>());
            }

            await _stageRepo.SaveAsync();
        }

        /// <summary>보스 전투 승리 시 클리어 스테이지 수를 1 증가하고 저장한다.</summary>
        public async UniTask IncrementClearedStageCount()
        {
            _stageRepo.IncrementClearedStageCount();
            await _stageRepo.SaveAsync();
        }

        public string GetBiomeBackground()
        {
            var stageSO = _stageMasterDataRepo.GetStageById(_stageRepo.RunData.CurrentStageId);
            return stageSO.BackgroundSpriteKey;
        }
    }
}
