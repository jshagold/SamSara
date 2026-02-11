using System;
using System.Collections.Generic;

public class GetCharacterSummaryUseCase
{
    private readonly string _logClass = $"[{nameof(GetCharacterSummaryUseCase)}]";

    private readonly ICharacterRepository _characterRepo;
    private readonly ICharacterMasterRepository _characterMasterRepo;
    private readonly IDailyStateRepository _dailyStateRepo;

    public event Action OnCharacterUpdated
    {
        add => _characterRepo.OnCharacterUpdated += value;
        remove => _characterRepo.OnCharacterUpdated -= value;
    }

    public GetCharacterSummaryUseCase(
        ICharacterRepository characterRepo,
        ICharacterMasterRepository characterMasterRepo,
        IDailyStateRepository dailyStateRepo)
    {
        _characterRepo = characterRepo ?? throw new ArgumentNullException(nameof(characterRepo));
        _characterMasterRepo = characterMasterRepo ?? throw new ArgumentNullException(nameof(characterMasterRepo));
        _dailyStateRepo = dailyStateRepo ?? throw new ArgumentNullException(nameof(dailyStateRepo));
    }

    public List<MainSceneCharacterSummaryInfo> GetCharacterSummaryList()
    {
        List<MainSceneCharacterSummaryInfo> dataList = new List<MainSceneCharacterSummaryInfo>();

        CharacterSaveData characterSaveData = _characterRepo.GetCharacterData();
        if (characterSaveData == null)
        {
            throw new InvalidOperationException($"{_logClass} saveData Load fail");
        }

        var characterMasterData = _characterMasterRepo.GetData(characterSaveData.CharacterId);
        if (characterMasterData == null)
        {
            throw new InvalidOperationException($"{_logClass} characterMasterData null - charId: {characterSaveData.CharacterId}");
        }

        var nodeData = characterMasterData.EvolutionNodes.Find(node => node.Id == characterSaveData.CurrentNodeId);
        if (nodeData == null)
        {
            throw new InvalidOperationException($"{_logClass} nodeData null - nodeId: {characterSaveData.CurrentNodeId}");
        }

        bool[] actionSlotList = _dailyStateRepo.GetActionSlot(charId: characterSaveData.CharacterId);
        if (actionSlotList == null)
        {
            throw new InvalidOperationException($"{_logClass} actionSlotList null - characterId: {characterSaveData.CharacterId}");
        }

        dataList.Add(new MainSceneCharacterSummaryInfo
        {
            CharacterId = characterSaveData.CharacterId,
            CurrentHp = characterSaveData.CurrentStats.Hp,
            MaxHp = nodeData.MaxStats.Hp,
            ActionFlags = (bool[])actionSlotList.Clone()
        });


        return dataList;
    }
}