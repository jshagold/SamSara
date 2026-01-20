using System;
using UnityEngine;

public class InventoryPresenter : IDisposable
{
    // Views
    private readonly InventoryView _inventoryView;

    // UseCases
    private readonly GetInventoryUseCase _getInventoryUseCase;

    // Resource Provider
    private readonly IItemResourceProvider _itemResourceProvider;

    public InventoryPresenter(
        InventoryView inventoryView, 
        GetInventoryUseCase getInventoryUseCase,
        IItemResourceProvider itemResourceProvider) 
    { 
        _inventoryView = inventoryView;
        _getInventoryUseCase = getInventoryUseCase;
        _itemResourceProvider = itemResourceProvider;
    }

    public void Initialize()
    {
        // 이벤트 구독
        _getInventoryUseCase.OnInventoryChanged += Refresh;

        // 화면 갱신
        Refresh();
    }

    public void Refresh()
    {
        _inventoryView.ClearList();

        InventoryInfo inventory = _getInventoryUseCase.Execute();

        foreach (var item in inventory.ItemList)
        {
            Sprite icon = _itemResourceProvider.GetIconSprite(itemId: item.Id);

            _inventoryView.AddItem(info: item, iconImage: icon, onClickItemAction: () => OnClickItem(item));
        }
    }

    private void OnClickItem(ItemInfo item)
    {
        Debug.Log($"아이템 클릭: {item.Id}");
    }

    public void Dispose()
    {
        if(_getInventoryUseCase != null)
        {
            _getInventoryUseCase.OnInventoryChanged -= Refresh;
        }
    }
}