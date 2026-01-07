using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemMasterRepository : IItemMasterRepository
{
    private string _logClass = "[ItemMasterRepository]";

    // 검색용 Dictionary
    private Dictionary<int, ItemMasterData> _itemDictionary = new Dictionary<int, ItemMasterData>();

    // 데이터를 불러올 경로 (Assets/Resources/MasterData/Items)
    private const string RESOURCE_PATH = "MasterData/Items";

    public List<ItemMasterData> GetAllData()
    {
        return _itemDictionary.Values.ToList();
    }

    public ItemMasterData GetData(int itemId)
    {
        if (_itemDictionary.TryGetValue(itemId, out var data))
        {
            return data;
        }

        Debug.LogWarning($"{_logClass} 존재하지 않는 ItemId 요청: {itemId}");
        return null;
    }

    public void LoadAll()
    {
        var assets = Resources.LoadAll<ItemMasterData>(RESOURCE_PATH);

        if(assets == null || assets.Length == 0)
        {
            Debug.LogWarning($"{_logClass} '{RESOURCE_PATH}' 경로에서 아이템 데이터를 찾을 수 없습니다.");
            return;
        }

        foreach(var asset in assets)
        {
            if(_itemDictionary.ContainsKey(asset.Id))
            {
                Debug.LogError($"{_logClass} 중복된 ItemId 발견: {asset.Id} ({asset.Name})");
                continue;
            }

            _itemDictionary.Add(asset.Id, asset);
        }

        Debug.Log($"{_logClass} 아이템 데이터 {_itemDictionary.Count}개 로드 완료.");
    }
}