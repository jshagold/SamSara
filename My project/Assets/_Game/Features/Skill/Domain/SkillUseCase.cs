using Samsara.Core.MasterData;

namespace Samsara.Features.Skill.Domain
{
    public class SkillUseCase
    {
        private readonly string _logClass = $"[{nameof(SkillUseCase)}]";

        private readonly ISkillMasterDataRepository _skillMasterDataRepo;

        public SkillUseCase(ISkillMasterDataRepository skillMasterDataRepo)
        {
            _skillMasterDataRepo = skillMasterDataRepo;
        }

        // ──────────────────────────────────────────────
        // MasterData Queries
        // ──────────────────────────────────────────────

        public SkillSO GetSkillById(int skillId)
        {
            return _skillMasterDataRepo.GetSkill(skillId);
        }

        public SkillSO[] GetSkillsByIds(int[] skillIds)
        {
            return _skillMasterDataRepo.GetSkillsByIds(skillIds);
        }

        public QTEPatternSO GetQTEPatternById(int patternId)
        {
            return _skillMasterDataRepo.GetQTEPattern(patternId);
        }

        public SkillSO[] GetSkillsForEvolutionNode(int[] skillIds)
        {
            return _skillMasterDataRepo.GetSkillsByIds(skillIds);
        }
    }
}
