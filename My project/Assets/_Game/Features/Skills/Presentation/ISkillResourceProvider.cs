using UnityEngine;

public interface ISkillResourceProvider
{
    Sprite GetIconSprite(int skillId);

    Sprite GetSkillEffect(int skillId);
}