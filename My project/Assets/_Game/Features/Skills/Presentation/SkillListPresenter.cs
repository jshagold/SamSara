using System;
using System.Collections.Generic;
using UnityEngine;

public class SkillListPresenter : IDisposable
{
    private readonly string _logClass = $"{nameof(SkillListPresenter)}";

    // Views
    private readonly SkillListView _skillListView;
    private readonly SkillDescPopupView _popupView;

    // UseCases
    private readonly GetSkillListUseCase _getSkillListUseCase;

    // Resource Provider
    private readonly ISkillResourceProvider _resourceProvider;

    public SkillListPresenter(
        SkillListView skillListView,
        SkillDescPopupView popupView,
        GetSkillListUseCase getSkillListUseCase,
        ISkillResourceProvider skillResourceProvider) 
    {
        _skillListView = skillListView;
        _popupView = popupView;
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

        Sprite icon = _resourceProvider.GetIconSprite(skillInfo.Id);
        // 프레임 이미지가 있다면 Provider에서 가져오기 (없으면 null)
        Sprite frame = null; // _resourceProvider.GetFrameSprite(skillInfo.Grade); 

        // 2. 텍스트 가져오기 (LocalizationUtils 사용 권장)
        // SkillInfo에 Key가 있다고 가정 (만약 번역된 텍스트가 바로 있다면 skillInfo.Name 사용)
        string name = LocalizationUtils.GetString(skillInfo.Name);
        string desc = LocalizationUtils.GetString(skillInfo.Desc);

        // 3. 팝업 열기
        _popupView.OpenPopup(name, desc, icon, frame);
    }

    public void Dispose()
    {
        if(_getSkillListUseCase != null)
        {
            _getSkillListUseCase.OnSkillListChanged -= Refresh;
        }
    }
}