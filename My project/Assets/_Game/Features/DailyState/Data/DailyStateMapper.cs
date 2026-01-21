using System.Collections.Generic;

public static class DailyStateMapper
{
    // SaveData(DTO) -> Domain
    public static DailyStateInfo ToDomain(this DailyStateSaveData dto)
    {
        if (dto == null)
        {
            return new DailyStateInfo(currentDay: 0, characterActionMap: new Dictionary<string, bool[]>());
        }
        return new DailyStateDto(currentDay: dto.currentDay, characterActionMap: dto.characterActionMap);
    }

    // Domain -> SaveData(DTO)
    public static DailyStateSaveData ToData(this DailyStateInfo domain)
    {
        return new DailyStateSaveData(currentDay: domain.currentDay, characterActionMap: domain.characterActionMap);
    }
}