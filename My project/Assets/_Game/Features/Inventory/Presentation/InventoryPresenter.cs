using System;
using UnityEngine;

public class InventoryPresenter : IDisposable
{
    // Views
    private readonly InventoryView _inventoryView;

    // UseCases
    private readonly GetInventoryUseCase _getInventoryUseCase;

    // MasterData
    IItemMasterRepository _itemMasterDataRepo;


    public InventoryPresenter(
        InventoryView inventoryView, 
        GetInventoryUseCase getInventoryUseCase,
        IItemMasterRepository itemMasterDataRepo) 
    { 
        _inventoryView = inventoryView;
        _getInventoryUseCase = getInventoryUseCase;
        _itemMasterDataRepo = itemMasterDataRepo;
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
            Sprite icon = _itemMasterDataRepo.GetData(itemId: item.Id).Icon;

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