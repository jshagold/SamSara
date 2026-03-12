using System;
using UnityEngine;

public class MasterDataManager
{
    private readonly string _logClass = $"[{nameof(MasterDataManager)}]";

    private bool _isInitialized = false;    // 초기화 여부

    // Repositories (injected by Bootstrapper; Constitution §2 – only Bootstrapper uses new)
    public IItemMasterRepository ItemRepo { get; }
    public IQtePatternMasterRepository QtePatternRepo { get; }
    public ISkillMasterRepository SkillRepo { get; }
    public ICharacterMasterRepository CharacterRepo { get; }

    public MasterDataManager(
        IItemMasterRepository itemRepo,
        IQtePatternMasterRepository qtePatternRepo,
        ISkillMasterRepository skillRepo,
        ICharacterMasterRepository characterRepo)
    {
        ItemRepo = itemRepo ?? throw new ArgumentNullException(nameof(itemRepo));
        QtePatternRepo = qtePatternRepo ?? throw new ArgumentNullException(nameof(qtePatternRepo));
        SkillRepo = skillRepo ?? throw new ArgumentNullException(nameof(skillRepo));
        CharacterRepo = characterRepo ?? throw new ArgumentNullException(nameof(characterRepo));
    }

    public void Initialize()
    {
        if (_isInitialized)
        {
            Debug.LogWarning($"{_logClass} already initialized");
            return;
        }

        Debug.Log($"{_logClass} 초기화 시작");

        try
        {
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

    private void LoadAllData()
    {
        // 순서 중요: QTE가 다른 데이터(Skill 등)보다 먼저 로드되어야 한다면 위로 올릴 것.
        // 현재 구조(ID 참조)상으로는 순서가 크게 상관없으나, 
        // SkillMapper에서 즉시 조회가 필요하다면 QteRepo를 먼저 로드하는 게 안전함.

        ItemRepo.LoadAll();
        QtePatternRepo.LoadAll();
        SkillRepo.LoadAll();
        CharacterRepo.LoadAll();
    }
}   