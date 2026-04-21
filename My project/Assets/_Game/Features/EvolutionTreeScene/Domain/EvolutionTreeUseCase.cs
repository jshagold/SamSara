using System;
using Cysharp.Threading.Tasks;
using Samsara.Core.MasterData;
using Samsara.Core.Tree;
using Samsara.Features.Character.Data;
using Samsara.Features.Character.Domain;
using Samsara.Features.Character.MasterData;
using Samsara.Features.Skill.Domain;

namespace Samsara.Features.EvolutionTreeScene.Domain
{
    public struct EvolutionTreeStatsData
    {
        public int Hp;
        public int Strength;
        public int Toughness;
        public int Agility;
    }

    public class EvolutionTreeUseCase
    {
        private readonly string _logClass = $"[{nameof(EvolutionTreeUseCase)}]";

        private readonly ICharacterRunRepository _characterRunRepo;
        private readonly ISkillMasterDataRepository _skillMasterDataRepo;
        private readonly EvolutionNodeSO[] _allNodes;

        public EvolutionTreeUseCase(
            ICharacterRunRepository characterRunRepo,
            ISkillMasterDataRepository skillMasterDataRepo,
            EvolutionNodeSO[] allNodes)
        {
            _characterRunRepo = characterRunRepo;
            _skillMasterDataRepo = skillMasterDataRepo;
            _allNodes = allNodes;
        }

        public string GetCurrentNodeId()
        {
            return _characterRunRepo.RunData.EvolutionNodeId;
        }

        public EvolutionTreeStatsData GetCurrentStats()
        {
            var data = _characterRunRepo.RunData;
            return new EvolutionTreeStatsData
            {
                Hp = data.Hp,
                Strength = data.Strength,
                Toughness = data.Toughness,
                Agility = data.Agility
            };
        }

        public EvolutionNodeSO[] GetAllNodes()
        {
            return _allNodes;
        }

        public SkillSO[] GetSkillsForNode(int[] skillIds)
        {
            return _skillMasterDataRepo.GetSkillsByIds(skillIds);
        }

        public EvolutionNodeSO FindCurrentNode()
        {
            var currentId = GetCurrentNodeId();
            foreach (var node in _allNodes)
            {
                if (node.NodeId == currentId)
                    return node;
            }

            throw new InvalidOperationException(
                $"{_logClass} Current evolution node not found: {currentId}");
        }

        public bool CheckUnlockConditions(EvolutionNodeSO node)
        {
            if (node.UnlockConditions == null || node.UnlockConditions.Length == 0)
                return true;

            var data = _characterRunRepo.RunData;
            foreach (var condition in node.UnlockConditions)
            {
                int currentValue = condition.StatType switch
                {
                    StatType.Hp => data.Hp,
                    StatType.Strength => data.Strength,
                    StatType.Toughness => data.Toughness,
                    StatType.Agility => data.Agility,
                    _ => 0
                };

                if (currentValue < condition.RequiredValue)
                    return false;
            }

            return true;
        }

        public NodeState ClassifyNodeState(EvolutionNodeSO node, EvolutionNodeSO currentNode)
        {
            if (node == currentNode)
                return NodeState.Current;

            bool isNextNode = false;
            if (currentNode.NextNodes != null)
            {
                foreach (var next in currentNode.NextNodes)
                {
                    if (next != null && next.NodeId == node.NodeId)
                    {
                        isNextNode = true;
                        break;
                    }
                }
            }

            if (isNextNode)
            {
                if (node.IsHidden && !CheckUnlockConditions(node))
                    return NodeState.Hidden;

                if (CheckUnlockConditions(node))
                    return NodeState.Evolvable;

                return NodeState.Reachable;
            }

            return NodeState.Locked;
        }

        public async UniTask ExecuteEvolutionAsync(EvolutionNodeSO targetNode)
        {
            var runData = _characterRunRepo.RunData;
            runData.EvolutionNodeId = targetNode.NodeId;
            runData.Hp = targetNode.BaseStats.Hp;
            runData.MaxHp = targetNode.BaseStats.Hp;
            runData.Strength = targetNode.BaseStats.Strength;
            runData.Toughness = targetNode.BaseStats.Toughness;
            runData.Agility = targetNode.BaseStats.Agility;
            runData.MaxActionPoints = targetNode.MaxActionPoints;
            _characterRunRepo.MarkDirty();
            await _characterRunRepo.SaveDataAsync();
        }

        public async UniTask ExecuteReincarnationAsync()
        {
            _characterRunRepo.RunData.IsReincarnationPending = true;
            _characterRunRepo.MarkDirty();
            await _characterRunRepo.SaveDataAsync();
        }
    }
}
