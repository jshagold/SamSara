using System.Collections.Generic;

public interface IItemMasterRepository
{
    /// <summary>
    /// 게임 시작 시 모든 아이템 데이터를 로드하여 캐싱합니다.
    /// </summary>
    void LoadAll();

    /// <summary>
    /// 아이템 ID로 마스터 데이터를 찾습니다.
    /// </summary>
    ItemMasterData GetData(int itemId);

    /// <summary>
    /// 전체 아이템 리스트를 반환합니다. (도감 등에서 사용)
    /// </summary>
    List<ItemMasterData> GetAllData();
}