using System.Collections.Generic;

public static class DailyStateMapper
{
    // SaveData(DTO) -> Domain
    public static DailyState ToDomain(this DailyStateData dto)
    {
        if (dto == null)
        {
            return new DailyState(currentDay: 0, characterActionMap: new Dictionary<string, bool[]>());
        }
        return new DailyState(currentDay: dto.currentDay, characterActionMap: dto.characterActionMap);
    }

    // Domain -> SaveData(DTO)
    public static DailyStateData ToData(this DailyState domain)
    {
        return new DailyStateData(currentDay: domain.currentDay, characterActionMap: domain.characterActionMap);
    }
}