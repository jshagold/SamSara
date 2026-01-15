using System;
using System.Collections.Generic;
using NUnit.Framework;

public class GetSkillListUseCase
{
    private readonly ICharacterRepository _characterRepo;

    public GetSkillListUseCase(ICharacterRepository characterRepo)
    {
        _characterRepo = characterRepo;
    }

    public List<SkillInfo> Execute()
    {
        CharacterInfo characterInfo = _characterRepo.GetCharacterData();
        EvolutionNodeInfo currentNodeInfo = characterInfo.CurrentEvolutionNode;

        return currentNodeInfo.SkillList;
    }

    public event Action OnSkillListChanged
    {
        add => _characterRepo.OnCharacterUpdated += value;
        remove => _characterRepo.OnCharacterUpdated -= value;
    }
}