using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class SkillListView : MonoBehaviour
{
    [Header("Root")]
    [SerializeField] private Transform _contentRoot;

    [Header("Prefab")]
    [SerializeField] private SkillIconView _skillIconPrefab;

    private List<SkillIconView> _spawnedSkillList = new List<SkillIconView>();

    public void ClearList()
    {
        foreach(var skillIconView in _spawnedSkillList)
        {
            if(skillIconView != null) Destroy(skillIconView);
        }
        _spawnedSkillList.Clear();
    }

    public void AddSkill(SkillInfo skillInfo, Sprite iconImage, Action onClickSkillIcon)
    {
        var newSkillIconView = Instantiate(_skillIconPrefab, _contentRoot);

        newSkillIconView.SetData(info: skillInfo, iconImage: iconImage);

        newSkillIconView.SetOnClickIcon(action: onClickSkillIcon);
    }
}