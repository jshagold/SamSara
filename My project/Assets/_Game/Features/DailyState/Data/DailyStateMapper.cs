using System.Collections.Generic;

public static class DailyStateMapper
{
    // SaveData(DTO) -> Domain
    public static DailyStateDto ToDomain(this DailyStateData dto)
    {
        if (dto == null)
        {
            return new DailyStateDto(currentDay: 0, characterActionMap: new Dictionary<string, bool[]>());
        }
        return new DailyStateDto(currentDay: dto.currentDay, characterActionMap: dto.characterActionMap);
    }

    // Domain -> SaveData(DTO)
    public static DailyStateData ToData(this DailyStateDto domain)
    {
        return new DailyStateData(currentDay: domain.currentDay, characterActionMap: domain.characterActionMap);
    }
}