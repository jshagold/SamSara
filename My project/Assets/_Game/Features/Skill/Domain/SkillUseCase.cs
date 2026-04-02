using System.Collections.Generic;
using Samsara.Core.MasterData;

namespace Samsara.Features.Skill.Domain
{
    public class SkillUseCase
    {
        private readonly string _logClass = $"[{nameof(SkillUseCase)}]";

        private readonly ISkillMasterDataRepository _skillMasterDataRepo;
        private readonly SkillRuntimeData _runtimeData;

        public SkillUseCase(ISkillMasterDataRepository skillMasterDataRepo)
        {
            _skillMasterDataRepo = skillMasterDataRepo;
            _runtimeData = new SkillRuntimeData();
        }

        // ──────────────────────────────────────────────
        // Combat Lifecycle
        // ──────────────────────────────────────────────

        public void InitializeBattle(int[] skillIds)
        {
            _runtimeData.Initialize(skillIds);
        }

        public void CleanupBattle()
        {
            _runtimeData.Clear();
        }

        // ──────────────────────────────────────────────
        // During Combat
        // ──────────────────────────────────────────────

        public bool CanUseSkill(int skillId, int currentHp)
        {
            if (_runtimeData.GetCooldown(skillId) != 0) return false;

            var skill = _skillMasterDataRepo.GetSkill(skillId);
            foreach (var cost in skill.Costs)
            {
                if (cost.CostType == CostType.Hp && currentHp <= cost.Value)
                    return false;
            }

            return true;
        }

        public SkillSO ConsumeSkill(int skillId)
        {
            var skill = _skillMasterDataRepo.GetSkill(skillId);

            foreach (var cost in skill.Costs)
            {
                if (cost.CostType == CostType.CoolDown)
                {
                    _runtimeData.SetCooldown(skillId, (int)cost.Value);
                    break;
                }
            }

            return skill;
        }

        public void TickCooldowns()
        {
            _runtimeData.TickAll();
        }

        public SkillSO[] GetAvailableSkills(int currentHp)
        {
            var allIds = _runtimeData.GetAllSkillIds();
            var available = new List<SkillSO>();

            foreach (var id in allIds)
            {
                if (CanUseSkill(id, currentHp))
                    available.Add(_skillMasterDataRepo.GetSkill(id));
            }

            return available.ToArray();
        }

        // ──────────────────────────────────────────────
        // Non-Combat
        // ──────────────────────────────────────────────

        public SkillSO[] GetSkillsForNode(int[] skillIds)
        {
            return _skillMasterDataRepo.GetSkillsByIds(skillIds);
        }

        public SkillSO GetSkill(int skillId)
        {
            return _skillMasterDataRepo.GetSkill(skillId);
        }

        public QTEPatternSO GetQTEPattern(int patternId)
        {
            return _skillMasterDataRepo.GetQTEPattern(patternId);
        }
    }
}
