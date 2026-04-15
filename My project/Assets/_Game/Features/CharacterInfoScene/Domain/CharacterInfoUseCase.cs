using System;
using Samsara.Core.MasterData;
using Samsara.Features.Character.Domain;
using Samsara.Features.Character.MasterData;
using Samsara.Features.Skill.Domain;
using UnityEngine;

namespace Samsara.Features.CharacterInfoScene.Domain
{
    public struct CharacterInfoStatsData
    {
        public int Hp;
        public int Strength;
        public int Toughness;
        public int Agility;
    }

    /// <summary>
    /// CharacterInfoScene 비즈니스 로직. 순수 C# — MonoBehaviour 금지.
    /// </summary>
    public class CharacterInfoUseCase
    {
        private readonly string _logClass = $"[{nameof(CharacterInfoUseCase)}]";

        private readonly ICharacterRunRepository    _characterRunRepo;
        private readonly ISkillMasterDataRepository _skillMasterDataRepo;
        private readonly EvolutionNodeSO[]          _evolutionNodes;

        public CharacterInfoUseCase(
            ICharacterRunRepository    characterRunRepo,
            ISkillMasterDataRepository skillMasterDataRepo,
            EvolutionNodeSO[]          evolutionNodes)
        {
            _characterRunRepo    = characterRunRepo;
            _skillMasterDataRepo = skillMasterDataRepo;
            _evolutionNodes      = evolutionNodes;
        }

        // ──────────────────────────────────────────────
        // Queries
        // ──────────────────────────────────────────────

        /// <summary>현재 스탯 스냅샷을 반환한다.</summary>
        public CharacterInfoStatsData GetCurrentStats()
        {
            var runData = _characterRunRepo.RunData;
            return new CharacterInfoStatsData
            {
                Hp        = runData.Hp,
                Strength  = runData.Strength,
                Toughness = runData.Toughness,
                Agility   = runData.Agility
            };
        }

        /// <summary>
        /// 현재 캐릭터의 EvolutionNodeSO를 반환한다.
        /// MasterData에 없으면 InvalidOperationException (§7 Fail Fast).
        /// </summary>
        public EvolutionNodeSO GetCurrentEvolutionNode()
        {
            var nodeId = _characterRunRepo.RunData.EvolutionNodeId;
            foreach (var node in _evolutionNodes)
            {
                if (node.NodeId == nodeId) return node;
            }
            throw new InvalidOperationException(
                $"{_logClass} EvolutionNodeSO를 찾을 수 없음: nodeId={nodeId}");
        }

        /// <summary>현재 진화 단계의 스킬 배열을 반환한다.</summary>
        public SkillSO[] GetSkills()
        {
            var node = GetCurrentEvolutionNode();
            return _skillMasterDataRepo.GetSkillsByIds(node.SkillIds);
        }
    }
}
