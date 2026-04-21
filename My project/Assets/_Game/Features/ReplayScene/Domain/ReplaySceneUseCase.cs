using System;
using Cysharp.Threading.Tasks;
using Samsara.Core.MasterData;
using Samsara.Core.Tree;
using Samsara.Features.Character.Data;
using Samsara.Features.Character.MasterData;
using Samsara.Features.Skill.Domain;

namespace Samsara.Features.ReplayScene.Domain
{
    public class ReplaySceneUseCase
    {
        private readonly string _logClass = $"[{nameof(ReplaySceneUseCase)}]";

        private readonly ICharacterAccountRepository _characterAccountRepo;
        private readonly ISkillMasterDataRepository  _skillMasterDataRepo;
        private readonly EvolutionNodeSO[]           _evolutionNodes;
        private readonly GameContext                 _gameContext;

        public ReplaySceneUseCase(
            ICharacterAccountRepository characterAccountRepo,
            ISkillMasterDataRepository  skillMasterDataRepo,
            EvolutionNodeSO[]           evolutionNodes,
            GameContext                 gameContext)
        {
            _characterAccountRepo = characterAccountRepo;
            _skillMasterDataRepo  = skillMasterDataRepo;
            _evolutionNodes       = evolutionNodes;
            _gameContext          = gameContext;
        }

        public EvolutionNodeSO[] GetAllNodes()
        {
            return _evolutionNodes;
        }

        public bool IsNodeUnlocked(EvolutionNodeSO node)
        {
            if (node == null) return false;
            var unlocked = _characterAccountRepo.AccountData.UnlockedEvolutionNodeIds;
            foreach (var id in unlocked)
            {
                if (id == node.NodeId) return true;
            }
            return false;
        }

        public NodeState ClassifyNodeStateForReplay(EvolutionNodeSO node)
        {
            if (node == null)
                throw new InvalidOperationException($"{_logClass} ClassifyNodeStateForReplay: node is null");

            if (!IsNodeUnlocked(node))
                return node.IsHidden ? NodeState.Hidden : NodeState.Locked;

            return NodeState.Selectable;
        }

        public SkillSO[] GetSkillsForNode(int[] skillIds)
        {
            return _skillMasterDataRepo.GetSkillsByIds(skillIds);
        }

        public async UniTask ExecuteReplayAsync(EvolutionNodeSO selectedNode)
        {
            if (selectedNode == null)
                throw new InvalidOperationException($"{_logClass} ExecuteReplayAsync: selectedNode is null");

            if (!IsNodeUnlocked(selectedNode))
                throw new InvalidOperationException(
                    $"{_logClass} ExecuteReplayAsync: selected node [{selectedNode.NodeId}] is not unlocked");

            if (!int.TryParse(selectedNode.NodeId, out int nodeIdInt))
                throw new InvalidOperationException(
                    $"{_logClass} ExecuteReplayAsync: NodeId [{selectedNode.NodeId}] is not parseable as int");

            await _gameContext.ResetRunForReplayAsync(nodeIdInt);
        }
    }
}
