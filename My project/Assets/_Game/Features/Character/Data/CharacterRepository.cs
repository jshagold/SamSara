using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

public class CharacterRepository : ICharacterRepository
{
    private readonly string _logClass = "[CharacterRepository]";
    private readonly string _filePath;
    private readonly NewGameConfig _newGameConfig;
    private readonly ICharacterMasterRepository _masterRepo;

    private UserCharacterData _cachedData;

    public event Action OnCharacterStatChanged;

    public CharacterRepository(NewGameConfig config, ICharacterMasterRepository masterRepo)
    {
        _newGameConfig = config;
        _filePath = Path.Combine(Application.persistentDataPath, "save_character_data.json");
        _masterRepo = masterRepo;
    }

    public EvolutionNodeInfo GetCurrentNode()
    {
        CheckDataIntegrity();

        var masterData = _masterRepo.GetData(characterId: _cachedData.CharacterId);
        if(masterData == null)
        {
            Debug.LogWarning($"{_logClass} GetCurrentNode - CharacterMasterData null characterId={_cachedData.CharacterId}");
            return null;
        }

        EvolutionNodeData currentNode = masterData.EvolutionNodes.Find(node => node.NodeId == _cachedData.CurrentNodeId);
        if(currentNode == null)
        {
            Debug.LogWarning($"{_logClass} GetCurrentNode - currentNode null CurrentNodeId={_cachedData.CurrentNodeId}");
            return null;
        }

        return currentNode.ToDomain();
    }

    public StatGroup GetCurrentStat()
    {
        CheckDataIntegrity();

        return _cachedData.CurrentStats;
    }

    public void UpdateStat(StatGroup stat)
    {
        CheckDataIntegrity();
        if (stat == null)
        {
            Debug.LogWarning($"{_logClass} UpdateStat 실패 - Stat null");
            return;
        }

        _cachedData.CurrentStats = stat;
        NotifyChanged();
    }

    public async UniTask LoadDataAsync()
    {
        if (_cachedData != null)
        {
            return;
        }

        if (!File.Exists(_filePath))
        {
            InitializeNewData();
            return;
        }

        try
        {
            string json = await UniTask.RunOnThreadPool(() => File.ReadAllText(_filePath));

            var loadData = JsonConvert.DeserializeObject<UserCharacterData>(json);
            if (loadData == null)
            {
                throw new System.InvalidOperationException($"{_logClass} [LoadDataAsync] 데이터가 null입니다. 파일 손상 의심.");
            }
            else
            {
                _cachedData = loadData;
            }

            Debug.Log($"{_logClass} 로드 완료");

            return;
        }
        catch (Exception e)
        {
            Debug.LogError($"{_logClass}[CRITICAL] 로드 실패: {e.Message}");
            throw;
        }
    }

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
        catch (System.Exception e)
        {
            Debug.LogError($"{_logClass}[IO Error] 파일 쓰기 실패!!: {e.Message}");
        }

        Debug.Log($"{_logClass} 저장 완료");
    }

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
        Debug.Log($"{_logClass} 신규데이터 생성 (초기 캐릭터: {_newGameConfig.StartingCharacterId})");

        int startCharacterId = _newGameConfig.StartingCharacterId;
        int startNodeId = _newGameConfig.StartingCharacterNodeId;

        CharacterMasterData masterData = _masterRepo.GetData(startCharacterId);
        List<EvolutionNodeData> nodeList = masterData.EvolutionNodes;
        EvolutionNodeData EvolutionNode = nodeList.Find(node => node.NodeId == startNodeId);

        if(masterData != null && EvolutionNode != null)
        {
            _cachedData = new UserCharacterData
            {
                CharacterId = masterData.CharacterId,
                CurrentNodeId = EvolutionNode.NodeId,
                CurrentStats = EvolutionNode.StartStats,
            };
        }
        else
        {
            throw new InvalidOperationException($"{_logClass} 데이터 초기화 실패! (CharacterMasterData 확인 필요 startCharacterId={startCharacterId})");
        }

        // 초기화 후 즉시 저장해서 파일 생성
        SaveDataAsync().Forget();
    }


    private void NotifyChanged()
    {
        OnCharacterStatChanged?.Invoke();
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