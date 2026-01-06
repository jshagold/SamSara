
using Cysharp.Threading.Tasks;
using System;

public interface IUserInventoryRepository
{
    int GetItemCount(int itemId);
    InventoryInfo GetInventory();

    void AddItem(int itemId, int count);
    void ConsumeItem(int itemId, int count);

    // =====
    UniTask<InventoryInfo> LoadDataAsync();
    UniTask SaveDataAsync();    // 일반 저장
    void SaveDataSync();        // [긴급] 앱 일시정지(Pause) 시 저장

    // ===
    // Event (AutoSaveManager 연동용)
    // ===
    // 데이터가 변할 때마다 이 이벤트가 울리면, 매니저가 MarkDirty()를 함
    event Action OnInventoryChanged;
}
