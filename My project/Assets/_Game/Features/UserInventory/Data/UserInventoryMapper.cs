public static class UserInventoryMapper
{
    // SaveData(DTO) -> Domain
    public static UserInventoryDto ToDomain(this UserInventoryData dto)
    {
        if (dto == null)
        {
            return new UserInventoryDto(money: 0);
        }
        return new UserInventoryDto(money: dto.money);
    }

    // Domain -> SaveData(DTO)
    public static UserInventoryData ToData(this UserInventoryDto domain)
    {
        return new UserInventoryData(money: domain.Money);        
    }
}