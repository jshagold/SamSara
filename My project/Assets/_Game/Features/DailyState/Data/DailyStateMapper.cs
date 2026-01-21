using System.Collections.Generic;

public static class DailyStateMapper
{
    // SaveData(DTO) -> Domain
    public static DailyStateInfo ToDomain(this DailyStateSaveData saveData)
    {
        return new DailyStateInfo
        {
            currentDay = saveData.currentDay, 
            characterActionMap = saveData.characterActionMap
        };
    }

    // Domain -> SaveData(DTO)
    public static DailyStateSaveData ToData(this DailyStateInfo domain)
    {
        return new DailyStateSaveData
        {
            currentDay = domain.currentDay, 
            characterActionMap = domain.characterActionMap
        };
    }
}