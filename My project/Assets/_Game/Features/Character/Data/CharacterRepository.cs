using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

public class CharacterRepository : ICharacterRepository
{
    private readonly string _logClass = "[CharacterRepository]";
    private readonly string _filePath;
    private readonly NewGameConfig _newGameConfig;
    private readonly ICharacterMasterRepository _characterMasterRepo;

    private CharacterSaveData _cachedData;

    public event Action OnCharacterUpdated;

    public CharacterRepository(NewGameConfig config, ICharacterMasterRepository characterMasterRepo)
    {
        _newGameConfig = config;
        _characterMasterRepo = characterMasterRepo;
        _filePath = Path.Combine(Application.persistentDataPath, "save_character_data.json");
    }

    public CharacterInfo GetCharacterInfo()
    {
        CheckDataIntegrity();

        var characterMasterData = _characterMasterRepo.GetData(characterId: _cachedData.CharacterId);
        if(characterMasterData == null)
        {
            throw new InvalidOperationException($"{_logClass}[GetCharacterInfo] MasterData 누락 (CharacterId: {_cachedData.CharacterId})");
        }

        var currentNode = characterMasterData.EvolutionNodes.FirstOrDefault(node => node.Id == _cachedData.CurrentNodeId);
        if(currentNode == null)
        {
            throw new InvalidOperationException($"{_logClass}[GetCharacterInfo] NodeData 누락 (CurrentNodeId: {_cachedData.CurrentNodeId})");
        }

        return new CharacterInfo
        {
            Id = _cachedData.CharacterId,
            Name = characterMasterData.Name,
            Description = characterMasterData.Desc,

            Portrait = currentNode.Portrait,
            EvolutionLevel = currentNode.Level,
            CurrentNodeId = currentNode.Id,

            Stats = _cachedData.CurrentStats.Clone(),

            SkillList = currentNode.SkillList
                .Select(skillMasterData => new SkillInfo
                {
                    Id = skillMasterData.Id,
                    Name = skillMasterData.Name,
                    Desc = skillMasterData.Desc,
                    Icon = skillMasterData.Icon,
                    EffectVisual = skillMasterData.EffectVisual,
                    DamageMultiplier = skillMasterData.DamageMultiplier,
                    CostList = new List<SkillCostData>(skillMasterData.CostList),
                    EffectList = new List<SkillEffectData>(skillMasterData.EffectList),
                    LinkedQtePatternId = skillMasterData.LinkedQtePattern != null
                        ? skillMasterData.LinkedQtePattern.Id
                        : -1,
                }).ToList(),
        };
    }

    public void ModifyStat(StatGroup stat)
    {
        CheckDataIntegrity();
        if (stat == null)
        {
            Debug.LogWarning($"{_logClass} UpdateStat 실패 - Stat null");
            return;
        }

        _cachedData.CurrentStats = stat.Clone();

        NotifyChanged();
    }

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

            var loadData = JsonConvert.DeserializeObject<CharacterSaveData>(json);
            if (loadData == null)
            {
                throw new InvalidOperationException($"{_logClass} [LoadDataAsync] 데이터가 null입니다. 파일 손상 의심.");
            }

            _cachedData = loadData;
            Debug.Log($"{_logClass} 로드 완료");
        }
        catch (Exception e)
        {
            Debug.LogError($"{_logClass}[LoadDataAsync] 로드 실패: {e.Message}");
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
            Debug.Log($"{_logClass} 저장 완료");
        }
        catch (Exception e)
        {
            Debug.LogError($"{_logClass}[IO Error] 파일 쓰기 실패!!: {e.Message}");
        }
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

        int startCharacterId = _newGameConfig.StartingCharacterId;
        int startNodeId = _newGameConfig.StartingCharacterNodeId;
        
        Debug.Log($"{_logClass} 신규 생성 시작 (CharacterId: {startCharacterId}, NodeID: {startNodeId})");

        CharacterMasterData characterMasterData = _characterMasterRepo.GetData(startCharacterId);
        if (characterMasterData == null)
        {
            throw new InvalidOperationException($"{_logClass} 초기화 실패! MasterData 없음 (CharacterId: {startCharacterId})");
        }

        EvolutionNodeData startNode = characterMasterData.EvolutionNodes.FirstOrDefault(node => node.Id == startNodeId);
        if(startNode == null)
        {
            throw new InvalidOperationException($"{_logClass} 초기화 실패! NodeID를 찾을수 없음 (NodeID: {startNodeId})");
        }

        _cachedData = new CharacterSaveData
        {
            CharacterId = characterMasterData.Id,
            CurrentNodeId = startNode.Id,

            CurrentStats = startNode.StartStats.Clone(),
        };

        // 초기화 후 즉시 저장해서 파일 생성
        SaveDataAsync().Forget();
    }


    private void NotifyChanged()
    {
        OnCharacterUpdated?.Invoke();
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