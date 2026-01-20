using UnityEngine;

public class SkillResourceProvider : ISkillResourceProvider
{
    private readonly string _logClass = $"{nameof(SkillResourceProvider)}";
    private readonly ISkillMasterRepository _masterRepo;
 
    public SkillResourceProvider(ISkillMasterRepository skillMasterRepo)
    {
        _masterRepo = skillMasterRepo;
    }

    public Sprite GetIconSprite(int skillId)
    {
        var masterData = _masterRepo.GetData(skillId);
        if(masterData == null)
        {
            Debug.LogWarning($"{_logClass} MasterData not found skillId:{skillId}");
            return null;
        }

        return masterData.Icon;
    }

    public Sprite GetSkillEffect(int skillId)
    {
        var masterData = _masterRepo.GetData(skillId);
        if (masterData == null)
        {
            Debug.LogWarning($"{_logClass} MasterData not found skillId:{skillId}");
            return null;
        }

        return masterData.EffectVisual;
    }
}