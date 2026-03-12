using System.Collections.Generic;

public interface ICharacterMasterRepository
{
    /// <summary>
    /// 게임 시작 시 모든 캐릭터 데이터를 로드하여 캐싱합니다.
    /// </summary>
    void LoadAll();

    /// <summary>
    /// 캐릭터 ID로 마스터 데이터를 찾습니다.
    /// </summary>
    CharacterMasterData GetData(int characterId);

    /// <summary>
    /// 전체 캐릭터 리스트를 반환합니다.
    /// </summary>
    List<CharacterMasterData> GetAllData();
}