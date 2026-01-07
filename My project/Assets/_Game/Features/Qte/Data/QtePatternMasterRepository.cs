using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QtePatternMasterRepository : IQtePatternMasterRepository
{
    private readonly string _logClass = "[QtePatternMasterRepository]";

    private Dictionary<int, QtePatternMasterData> _qtePatternDictionary = new Dictionary<int, QtePatternMasterData>();
    // (Assets/Resources/MasterData/QtePatterns)
    private const string RESOURCE_PATH = "MasterData/QtePatterns";

    public List<QtePatternMasterData> GetAllData()
    {
        return _qtePatternDictionary.Values.ToList();
    }

    public QtePatternMasterData GetData(int qtePatternId)
    {
        if(_qtePatternDictionary.TryGetValue(qtePatternId, out var pattern))
        {
            return pattern;
        }

        Debug.LogWarning($"{_logClass} 존재하지 않는 QtePatternId 요청: {qtePatternId}");
        return null;
    }

    public void LoadAll()
    {
        var assets = Resources.LoadAll<QtePatternMasterData>(RESOURCE_PATH);

        foreach(var asset in assets)
        {
            if(_qtePatternDictionary.ContainsKey(asset.Id))
            {
                Debug.LogError($"{_logClass} 중복된 QtePatternId 발견: {asset.Id} ({asset.Name})");
                continue;
            }
            else
            {
                _qtePatternDictionary.Add(asset.Id, asset);
            }
        }

        Debug.Log($"{_logClass} Qte 패턴 데이터 {_qtePatternDictionary.Count}개 로드 완료.");
    }
}