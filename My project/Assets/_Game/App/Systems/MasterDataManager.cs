

using System;
using UnityEngine;

public class MasterDataManager
{
    private readonly string _logClass = "[MasterDataManager]";

    private bool _isInitialized = false;    // 초기화 여부

    // Repository
    public IItemMasterRepository ItemRepo { get; private set; }
    public ISkillMasterRepository SkillRepo { get; private set; }
    public ICharacterMasterRepository CharacterRepo { get; private set; }

    public void Initialize()
    {
        if(_isInitialized)
        {
            Debug.LogWarning($"{_logClass} alreadey initialized");
            return;
        }

        Debug.Log($"{_logClass} 초기화 시작");

        try
        {
            CreateRepositories();

            LoadAllData();

            _isInitialized = true;
            Debug.Log($"{_logClass} 데이터 초기화 및 로드 완료");
        }
        catch (Exception e)
        {
            Debug.LogError($"{_logClass} [CRITICAL] 로드 실패. 게임을 진행할 수 없습니다. Error: {e.Message}");
            throw;
        }
    }

    private void CreateRepositories()
    {
        ItemRepo = new ItemMasterRepository();
        SkillRepo = new SkillMasterRepository();
        CharacterRepo = new CharacterMasterRepository();
    }

    private void LoadAllData()
    {
        ItemRepo.LoadAll();
        SkillRepo.LoadAll();
        CharacterRepo.LoadAll();
    }
}   