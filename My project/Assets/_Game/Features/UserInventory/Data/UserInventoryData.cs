using Newtonsoft.Json;
using System;

[Serializable]
public class UserInventoryData
{
    public int money;

    [JsonConstructor]
    public UserInventoryData(int money)
    {
        this.money = money;
    }   
}