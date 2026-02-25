using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SkillMasterRepository : ISkillMasterRepository
{
    private string _logClass = $"[{nameof(SkillMasterRepository)}]";

    // 검색용 Dictionary
    private Dictionary<int, SkillMasterData> _skillDictionary = new Dictionary<int, SkillMasterData>();

    // 데이터를 불러올 경로 (Assets/Resources/MasterData/Skills)
    private const string RESOURCE_PATH = "MasterData/Skills";

    public List<SkillMasterData> GetAllData()
    {
        return _skillDictionary.Values.ToList();
    }

    public SkillMasterData GetData(int skillId)
    {
        if (_skillDictionary.TryGetValue(skillId, out var data))
        {
            return data;
        }

        Debug.LogWarning($"{_logClass} 존재하지 않는 SkillId 요청: {skillId}");
        return null;
    }

    public void LoadAll()
    {
        var assets = Resources.LoadAll<SkillMasterData>(RESOURCE_PATH);

        if (assets == null || assets.Length == 0)
        {
            Debug.LogWarning($"{_logClass} '{RESOURCE_PATH}' 경로에서 스킬 데이터를 찾을 수 없습니다.");
            return;
        }

        foreach (var asset in assets)
        {
            if (_skillDictionary.ContainsKey(asset.Id))
            {
                Debug.LogError($"{_logClass} 중복된 SkillId 발견: {asset.Id} ({asset.Name})");
                continue;
            }

            _skillDictionary.Add(asset.Id, asset);
        }

        Debug.Log($"{_logClass} 스킬 데이터 {_skillDictionary.Count}개 로드 완료.");
    }
}