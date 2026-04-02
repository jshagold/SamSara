using Cysharp.Threading.Tasks;
using Samsara.Core.MasterData;

namespace Samsara.Features.Skill.Domain
{
    public interface ISkillMasterDataRepository
    {
        UniTask LoadAsync();
        SkillSO GetSkill(int skillId);
        QTEPatternSO GetQTEPattern(int patternId);
        SkillSO[] GetAllSkills();
        SkillSO[] GetSkillsByIds(int[] skillIds);
    }
}
