using Newtonsoft.Json;
using UnityEngine;

public class UserInventoryRepository : IUserInventroyRepository
{
    public UserInventory GetInventory()
    {
        if(PlayerPrefs.HasKey("UserInventory"))
        {
            string json = PlayerPrefs.GetString("UserInventory");
            return JsonConvert.DeserializeObject<UserInventory>(json);
        }

        return new UserInventory { Money = 10000 };
    }

    public void SaveInventory(UserInventory inventory)
    {
        throw new System.NotImplementedException();
    }
}
