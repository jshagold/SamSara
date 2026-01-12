using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryView : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private Transform _contentRoot;

    [Header("Prefab")]
    [SerializeField] private ItemIconView _itemIconPrefab;

    private List<ItemIconView> _spawnedItemList = new List<ItemIconView>();

    public void ClearList()
    {
        foreach(var item in _spawnedItemList)
        {
            if(item != null) Destroy(item.gameObject);
        }
        _spawnedItemList.Clear();

        _scrollRect.horizontalNormalizedPosition = 0f;
    }

    public void AddItem(ItemInfo info, Sprite iconImage, Action onClickItemAction)
    {
        var newItemView = Instantiate(_itemIconPrefab, _contentRoot);
        
        newItemView.SetData(info: info, iconImage: iconImage);
        newItemView.SetOnClickIcon(onClickItemAction);
        _spawnedItemList.Add(newItemView);
    }
}