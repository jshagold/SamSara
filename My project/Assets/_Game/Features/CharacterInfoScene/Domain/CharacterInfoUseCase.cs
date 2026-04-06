using System;
using Samsara.Core.MasterData;
using Samsara.Features.Character.Domain;
using Samsara.Features.Character.MasterData;
using Samsara.Features.Skill.Domain;

namespace Samsara.Features.CharacterInfoScene.Domain
{
    public struct CharacterInfoStatsData
    {
        public int Hp;
        public int Strength;
        public int Toughness;
        public int Agility;
    }

    public class CharacterInfoUseCase
    {
        private readonly string _logClass = $"[{nameof(CharacterInfoUseCase)}]";

        private readonly ICharacterRunRepository     _characterRunRepo;
        private readonly ISkillMasterDataRepository  _skillMasterDataRepo;
        private readonly EvolutionNodeSO[]           _evolutionNodes;

        public CharacterInfoUseCase(
            ICharacterRunRepository    characterRunRepo,
            ISkillMasterDataRepository skillMasterDataRepo,
            EvolutionNodeSO[]          evolutionNodes)
        {
            _characterRunRepo    = characterRunRepo;
            _skillMasterDataRepo = skillMasterDataRepo;
            _evolutionNodes      = evolutionNodes;
        }

        public CharacterInfoStatsData GetCurrentStats()
        {
            var runData = _characterRunRepo.RunData;
            return new CharacterInfoStatsData
            {
                Hp         = runData.Hp,
                Strength   = runData.Strength,
                Toughness  = runData.Toughness,
                Agility    = runData.Agility
            };
        }

        public EvolutionNodeSO GetCurrentEvolutionNode()
        {
            var nodeId = _characterRunRepo.RunData.EvolutionNodeId;
            foreach (var node in _evolutionNodes)
            {
                if (node.NodeId == nodeId) return node;
            }
            throw new InvalidOperationException(
                $"{_logClass} EvolutionNode not found for id: {nodeId}");
        }

        public SkillSO[] GetSkills()
        {
            var node = GetCurrentEvolutionNode();
            return _skillMasterDataRepo.GetSkillsByIds(node.SkillIds);
        }
    }
}
