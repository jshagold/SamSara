public static class UserInventoryMapper
{
    // SaveData(DTO) -> Domain
    public static UserInventory ToDomain(this UserInventoryData dto)
    {
        if (dto == null)
        {
            return new UserInventory(money: 0);
        }
        return new UserInventory(money: dto.money);
    }

    // Domain -> SaveData(DTO)
    public static UserInventoryData ToData(this UserInventory domain)
    {
        return new UserInventoryData(money: domain.Money);        
    }
}