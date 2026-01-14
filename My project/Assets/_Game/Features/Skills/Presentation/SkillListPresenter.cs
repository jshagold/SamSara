using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillListPresenter : IDisposable
{
    // Views
    private readonly SkillListView _skillListView;

    // UseCases
    private readonly GetSkillListUseCase _getSkillListUseCase;

    // SkillMasterData
    private readonly ISkillMasterRepository _skillMasterRepo;

    public SkillListPresenter(
        SkillListView skillListView,
        GetSkillListUseCase getSkillListUseCase,
        ISkillMasterRepository skillMasterRepo) 
    {
        _skillListView = skillListView;
        _getSkillListUseCase = getSkillListUseCase;
        _skillMasterRepo = skillMasterRepo;
    }

    public void Initialize()
    {
        _getSkillListUseCase.OnSkillListChanged += Refresh;

        Refresh();
    }

    private void Refresh()
    {
        _skillListView.ClearList();

        List<SkillInfo> skillList = _getSkillListUseCase.Execute();
        foreach (SkillInfo skillInfo in skillList)
        {
            SkillMasterData skillMasterData = _skillMasterRepo.GetData(skillId: skillInfo.Id);
            Sprite icon = skillMasterData.Icon;

            _skillListView.AddSkill(skillInfo:skillInfo, iconImage: icon, onClickSkillIcon: () => OnClickSkill(skillInfo));
        }
    }

    private void OnClickSkill(SkillInfo skillInfo)
    {
        Debug.Log($"스킬 클릭: {skillInfo.Id} {skillInfo.Name}");
    }

    public void Dispose()
    {
        _getSkillListUseCase.OnSkillListChanged -= Refresh;
    }
}