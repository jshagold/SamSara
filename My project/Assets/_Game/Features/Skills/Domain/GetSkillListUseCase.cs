using System;
using System.Collections.Generic;
using System.Linq;

public class GetSkillListUseCase
{
    private readonly string _logClass = $"{nameof(GetSkillListUseCase)}";

    private readonly ICharacterRepository _characterRepo;
    private readonly ICharacterMasterRepository _characterMasterRepo;

    public GetSkillListUseCase(
        ICharacterRepository characterRepo,
        ICharacterMasterRepository characterMasterRepo)
    {
        _characterRepo = characterRepo;
        _characterMasterRepo = characterMasterRepo;
    }

    public List<SkillInfo> Execute()
    {
        var characterSaveData = _characterRepo.GetCharacterData();
        if (characterSaveData == null)
        {
            throw new InvalidOperationException($"{_logClass} saveData Load fail");
        }

        var characterMasterData = _characterMasterRepo.GetData(characterSaveData.CharacterId);
        if (characterMasterData == null)
        {
            throw new InvalidOperationException($"{_logClass} characterMasterData null - charId: {characterSaveData.CharacterId}");
        }

        var nodeMasterData = characterMasterData.EvolutionNodes.Find(node => node.Id == characterSaveData.CurrentNodeId);
        if (nodeMasterData == null)
        {
            throw new InvalidOperationException($"{_logClass} nodeData null - nodeId: {characterSaveData.CurrentNodeId}");
        }

        List<SkillInfo> skillList = nodeMasterData.SkillList.Select(skillMasterData => skillMasterData.ToDomain()).ToList();

        return skillList;
    }

    public event Action OnSkillListChanged
    {
        add => _characterRepo.OnCharacterUpdated += value;
        remove => _characterRepo.OnCharacterUpdated -= value;
    }
}