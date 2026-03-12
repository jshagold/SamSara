using System.Collections.Generic;

public interface IQtePatternMasterRepository
{
    /// <summary>
    /// 게임 시작 시 모든 스킬 데이터를 로드하여 캐싱합니다.
    /// </summary>
    void LoadAll();

    /// <summary>
    /// 스킬 ID로 마스터 데이터를 찾습니다.
    /// </summary>
    QtePatternMasterData GetData(int qtePatternId);

    /// <summary>
    /// 전체 스킬 리스트를 반환합니다.
    /// </summary>
    List<QtePatternMasterData> GetAllData();
}