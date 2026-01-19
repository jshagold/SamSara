using System.Collections.Generic;
using NUnit.Framework;

public class StatListPresenter
{
    private readonly StatListView _statListView;

    public StatListPresenter(StatListView statListView) 
    {
        _statListView = statListView;
    }

    public void Refresh(List<StatDisplayInfo> statList)
    {
        foreach(var stat in statList)
        {
            stat.Label = GetStatLabel(stat.Type);
        }

        // TODO string 입력해야함
        string statListLabel = LocalizationUtils.GetString("UI_TABLE", "Stat_List_Label");
        _statListView.SetData(statList: statList, boxLabel: statListLabel);
    }

    private string GetStatLabel(StatType type)
    {
        return type switch
        {
            // TOOD Key값 입력해야함
            StatType.Hp => "Stat_Hp",
            StatType.Strength => "Stat_Strength",
            StatType.Toughness => "Stat_Toughness",
            StatType.Agility => "Stat_Agility",
            _ => type.ToString(),
        };
    }
}