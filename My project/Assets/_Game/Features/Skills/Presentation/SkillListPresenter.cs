using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillListPresenter : IDisposable
{
    private readonly string _logClass = $"{nameof(SkillListPresenter)}";

    // Views
    private readonly SkillListView _skillListView;

    // UseCases
    private readonly GetSkillListUseCase _getSkillListUseCase;

    // Resource Provider
    private readonly ISkillResourceProvider _resourceProvider;

    public SkillListPresenter(
        SkillListView skillListView,
        GetSkillListUseCase getSkillListUseCase,
        ISkillResourceProvider skillResourceProvider) 
    {
        _skillListView = skillListView;
        _getSkillListUseCase = getSkillListUseCase;
        _resourceProvider = skillResourceProvider;
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
            Sprite icon = _resourceProvider.GetIconSprite(skillId: skillInfo.Id);

            _skillListView.AddSkill(skillInfo:skillInfo, iconImage: icon, onClickSkillIcon: () => OnClickSkill(skillInfo));
        }
    }

    private void OnClickSkill(SkillInfo skillInfo)
    {
        Debug.Log($"스킬 클릭: {skillInfo.Id} {skillInfo.Name}");
        //TODO 설명툴팁View 추가
    }

    public void Dispose()
    {
        if(_getSkillListUseCase != null)
        {
            _getSkillListUseCase.OnSkillListChanged -= Refresh;
        }
    }
}