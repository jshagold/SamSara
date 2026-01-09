using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CharacterMasterRepository : ICharacterMasterRepository
{
    private readonly string _logClass = $"[{nameof(CharacterMasterRepository)}]";

    // 검색용 Dictionary
    private Dictionary<int, CharacterMasterData> _characterDictionary = new Dictionary<int, CharacterMasterData>();

    // 데이터를 불러올 경로 (Assets/Resources/MasterData/Characters)
    private const string RESOURCE_PATH = "MasterData/Characters";

    public List<CharacterMasterData> GetAllData()
    {
        return _characterDictionary.Values.ToList();
    }

    public CharacterMasterData GetData(int characterId)
    {
        if(_characterDictionary.TryGetValue(characterId, out var data))
        {
            return data;
        }

        Debug.LogWarning($"{_logClass} 존재하지 않는 characterId 요청: {characterId}");
        return null;
    }

    public void LoadAll()
    {
        var assets = Resources.LoadAll<CharacterMasterData>(RESOURCE_PATH);
        if (assets == null || assets.Length == 0)
        {
            Debug.LogWarning($"{_logClass} '{RESOURCE_PATH}' 경로에서 아이템 데이터를 찾을 수 없습니다.");
            return;
        }

        foreach (var asset in assets)
        {
            if (_characterDictionary.ContainsKey(asset.Id))
            {
                Debug.LogError($"{_logClass} 중복된 CharacterId 발견: {asset.Id} ({asset.Name})");
                continue;
            }

            _characterDictionary.Add(asset.Id, asset);
        }

        Debug.Log($"{_logClass} 캐릭터 데이터 {_characterDictionary.Count}개 로드 완료.");
    }
}