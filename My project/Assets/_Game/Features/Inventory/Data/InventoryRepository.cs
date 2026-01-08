using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;


public class InventoryRepository : IInventoryRepository
{
    private readonly string _logClass = "[InventoryRepository]";
    private readonly string _filePath;
    private readonly NewGameConfig _newGameConfig;
    private readonly IItemMasterRepository _itemMasterRepo;

    private InventorySaveData _cachedData;

    // 데이터 변경 알림 이벤트
    public event Action OnInventoryChanged;

    public InventoryRepository(NewGameConfig config, IItemMasterRepository itemMasterRepository)
    {
        _newGameConfig = config;
        _itemMasterRepo = itemMasterRepository;
        // 안드로이드/IOS/PC 공용 경로
        _filePath = Path.Combine(Application.persistentDataPath, "save_inventory_data.json");
    }

    // ======================================================================================================
    // Read
    // ======================================================================================================
    public int GetItemCount(int itemId)
    {
        CheckDataIntegrity();

        var item = _cachedData.ItemList.FirstOrDefault(item => item.Id == itemId);
        return item?.Count ?? 0;
    }

    public InventoryInfo GetInventory()
    {
        CheckDataIntegrity();

        var domainItemList = new List<ItemInfo>();

        foreach(var saveItem in _cachedData.ItemList)
        {
            var masterItem = _itemMasterRepo.GetData(saveItem.Id);

            if(masterItem != null)
            {
                domainItemList.Add(masterItem.ToDomain(count: saveItem.Count));
            }
        }

        return new InventoryInfo
        {
            ItemList = domainItemList,
        };
    }


    // ======================================================================================================
    // Update
    // ======================================================================================================
    public void AddItem(int itemId, int count)
    {
        CheckDataIntegrity();
        if (count <= 0) return;

        if (_itemMasterRepo.GetData(itemId: itemId) == null)
        {
            throw new InvalidOperationException($"{_logClass} AddItem 실패 - 존재하지 않는 ItemID: {itemId}");
        }

        var item = _cachedData.ItemList.FirstOrDefault(item => item.Id == itemId);
        if(item == null)
        {
            _cachedData.ItemList.Add(
                new ItemSaveData
                {
                    Id = itemId, 
                    Count = count
                }
            );
        }
        else
        {
             item.Count += count;
        }

        NotifyChanged();
    }

    public void ConsumeItem(int itemId, int count)
    {
        CheckDataIntegrity();

        if(count <= 0) return;

        var item = _cachedData.ItemList.FirstOrDefault(item => item.Id == itemId);
        if (item == null || item.Count < count)
        {
            Debug.LogWarning($"{_logClass} ConsumeItem 실패 - 보유량 부족 (ID: {itemId}, 보유: {item?.Count ?? 0}, 필요: {count})");
            return;
        }

        item.Count -= count;

        // [옵션] 개수가 0이 되면 리스트에서 삭제할지 여부는 기획에 따라 결정

        NotifyChanged();
    }


    // ======================================================================================================
    // Load / Save
    // ======================================================================================================
    public async UniTask LoadDataAsync()
    {
        if(_cachedData != null) return;

        if (!File.Exists(_filePath))
        {
            InitializeNewData();
            return;
        }

        try
        {
            string json = await UniTask.RunOnThreadPool(() => File.ReadAllText(_filePath));

            var loadData = JsonConvert.DeserializeObject<InventorySaveData>(json);
            if (loadData == null)
            {
                throw new InvalidOperationException("[LoadDataAsync] 데이터가 null입니다. 파일 손상 의심.");
            }
            else
            {
                _cachedData = loadData;

                // 리스트가 null일 경우 방어 코드 (생성자 호출 없이 역직렬화될 경우 대비)
                if (_cachedData.ItemList == null) _cachedData.ItemList = new List<ItemSaveData>();
            }

            Debug.Log($"{_logClass} 로드 완료");
        }
        catch (Exception e)
        {
            Debug.LogError($"{_logClass}[CRITICAL] 로드 실패: {e.Message}");
            throw;
        }
    }

    // 정보 저장 - 비동기
    public async UniTask SaveDataAsync()
    {
        CheckDataIntegrity();

        // 스레드 풀에서 저장. (게임 멈춤 방지)
        try
        {
            await UniTask.RunOnThreadPool(() =>
            {
                string json = JsonConvert.SerializeObject(_cachedData, Formatting.Indented);
                File.WriteAllText(_filePath, json);
            });
        }
        catch (Exception e)
        {
            Debug.LogError($"{_logClass}[IO Error] 파일 쓰기 실패!!: {e.Message}");
        }

        Debug.Log($"{_logClass} 저장 완료");
    }

    // 긴급 저장 (OnApplicationPause 용) - 동기
    public void SaveDataSync()
    {
        if (_cachedData == null)
        {
            Debug.LogWarning($"{_logClass}[Save Skip] 로드된 데이터가 없어서 강제 저장 스킵");
            return;
        }

        try
        {
            // 메인 스레드에서 즉시 씀
            string json = JsonConvert.SerializeObject(_cachedData);
            File.WriteAllText(_filePath, json);
            Debug.Log($"{_logClass} 동기 저장 완료");
        }
        catch (Exception e)
        {
            Debug.LogError($"{_logClass} 동기 저장 실패: {e.Message}");
        }
    }


    // ======================================================================================================
    // 내부 유틸리티
    // ======================================================================================================
    private void InitializeNewData()
    {
        Debug.Log($"{_logClass} 신규데이터 생성 (초기 자금: {_newGameConfig.InitialMoney})");

        _cachedData = new InventorySaveData();

        if(_newGameConfig.InitialMoney > 0)
        {
            _cachedData.ItemList.Add(new ItemSaveData
            {
                Id = ItemConstants.MONEY_ID, 
                Count = _newGameConfig.InitialMoney
            });
        }

        // 초기화 후 즉시 저장해서 파일 생성
        SaveDataAsync().Forget();
    }

    private void NotifyChanged()
    {
        OnInventoryChanged?.Invoke();
    }

    // 데이터 무결성 체크 (Fail Fast)
    private void CheckDataIntegrity()
    {
        if (_cachedData == null)
        {
            throw new InvalidOperationException($"{_logClass} 데이터가 로드되지 않았습니다! (Bootstrapper 확인 필요)");
        }
    }
}
