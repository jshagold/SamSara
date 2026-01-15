using System;
using System.Collections.Generic;

public class GetCharacterDetailUseCase
{
    private readonly string _logClass = $"{nameof(GetCharacterDetailUseCase)}";

    private readonly ICharacterRepository _characterRepo;
    private readonly ICharacterMasterRepository _masterRepo;

    public GetCharacterDetailUseCase(
        ICharacterRepository characterRepo,
        ICharacterMasterRepository characterMasterRepo)
    {
        _characterRepo = characterRepo;
        _masterRepo = characterMasterRepo;
    }

    public (CharacterInfo Info, List<StatDisplayInfo> Stats) Execute()
    {
        var saveData = _characterRepo.GetCharacterData();
        if(saveData == null)
        {
            throw new InvalidOperationException($"{_logClass} saveData Load fail");
        }

        var characterMasterData = _masterRepo.GetData(saveData.CharacterId);
        if(characterMasterData == null)
        {
            throw new InvalidOperationException($"{_logClass} characterMasterData null - charId: {saveData.CharacterId}");
        }

        var nodeData = characterMasterData.EvolutionNodes.Find(node => node.Id == saveData.CurrentNodeId);
        if (nodeData == null)
        {
            throw new InvalidOperationException($"{_logClass} nodeData null - nodeId: {saveData.CurrentNodeId}");
        }

        var characterInfo = new CharacterInfo
        {
            Id = saveData.CharacterId,
            Name = characterMasterData.Name,
            Description = characterMasterData.Desc,
            CurrentEvolutionNode = nodeData.ToDomain(),
            CurrentStats = saveData.CurrentStats.Clone()
        };

        var statList = new List<StatDisplayInfo>();

        statList.Add(CreateStatInfo(
            type: StatType.Hp,
            currentStat: saveData.CurrentStats.Hp, 
            startStat: nodeData.StartStats.Hp, 
            maxStat: nodeData.MaxStats.Hp
        ));

        statList.Add(CreateStatInfo(
            type: StatType.Strength,
            currentStat: saveData.CurrentStats.Strength,
            startStat: nodeData.StartStats.Strength,
            maxStat: nodeData.MaxStats.Strength
        ));

        statList.Add(CreateStatInfo(
            type: StatType.Toughness,
            currentStat: saveData.CurrentStats.Toughness,
            startStat: nodeData.StartStats.Toughness,
            maxStat: nodeData.MaxStats.Toughness
        ));

        statList.Add(CreateStatInfo(
            type: StatType.Agility,
            currentStat: saveData.CurrentStats.Agility,
            startStat: nodeData.StartStats.Agility,
            maxStat: nodeData.MaxStats.Agility
        ));

        return (characterInfo, statList);
    }

    private StatDisplayInfo CreateStatInfo(StatType type, StatInfo currentStat, StatInfo startStat, StatInfo maxStat)
    {
        return new StatDisplayInfo
        {
            Type = type,
            Label = GetStatLabel(type),
            CurrentValue = currentStat.Value,
            StartValue = startStat.Value,
            MaxValue = maxStat.Value
        };
    }


    // TODO LocalizationManager등으로 변환해야함.
    private string GetStatLabel(StatType type)
    {
        return type switch
        {
            StatType.Hp => "",
            StatType.Strength => "",
            StatType.Toughness => "",
            StatType.Agility => "",
            _ => type.ToString(),
        };
    }
}