public static class UserInventoryMapper
{
    // SaveData(DTO) -> Domain
    public static UserInventoryInfo ToDomain(this UserInventoryData dto)
    {
        if (dto == null)
        {
            return new UserInventoryInfo(money: 0);
        }
        return new UserInventoryInfo(money: dto.money);
    }

    // Domain -> SaveData(DTO)
    public static UserInventoryData ToData(this UserInventoryInfo domain)
    {
        return new UserInventoryData(money: domain.Money);        
    }
}