using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
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
        throw new NotImplementedException();
    }

    public StatGroup GetCurrentStat()
    {
        throw new NotImplementedException();
    }

    public void UpdateStat(StatGroup stat)
    {
        throw new NotImplementedException();
    }

    public UniTask LoadDataAsync()
    {
        throw new NotImplementedException();
    }

    public UniTask SaveDataAsync()
    {
        throw new NotImplementedException();
    }

    public void SaveDataSync()
    {
        throw new NotImplementedException();
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

        if(masterData != null)
        {
            _cachedData = new UserCharacterData
            {
                CharacterId = masterData.CharacterId,
                CurrentNodeId = masterData.,
                CurrentStats = ,
            }
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