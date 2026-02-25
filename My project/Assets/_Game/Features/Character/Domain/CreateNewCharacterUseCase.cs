using System;
using Core.ErrorHandling;
using UnityEngine;

public class CreateNewCharacterUseCase
{
    private readonly string _logClass = $"[{nameof(CreateNewCharacterUseCase)}]";

    private readonly ICharacterRepository _characterRepo;
    private readonly ICharacterMasterRepository _characterMasterRepo;
    private readonly NewGameConfig _newGameConfig;
    private readonly IStabilityFlag _stabilityFlag;

    public CreateNewCharacterUseCase(
        ICharacterRepository characterRepo,
        ICharacterMasterRepository characterMasterRepo,
        NewGameConfig newGameConfig,
        IStabilityFlag stabilityFlag = null)
    {
        _characterRepo = characterRepo;
        _characterMasterRepo = characterMasterRepo;
        _newGameConfig = newGameConfig;
        _stabilityFlag = stabilityFlag;
    }

    /// <summary>True when no character save exists and new game data should be created.</summary>
    public bool IsNewGameRequired() => !_characterRepo.HasSaveData();

    public void Execute()
    {
        if (_stabilityFlag != null && !_stabilityFlag.IsSaveAllowed) return;

        int charId = _newGameConfig.StartingCharacterId;
        int nodeId = _newGameConfig.StartingCharacterNodeId;

        CharacterMasterData masterData = _characterMasterRepo.GetData(charId);
        if(masterData == null)
        {
            throw new InvalidOperationException($"{_logClass} masterData null - charId:{charId}");
        }

        EvolutionNodeData startNode = masterData.EvolutionNodes.Find(node => node.Id == nodeId);
        if(startNode == null)
        {
            throw new InvalidOperationException($"{_logClass} startNode null - nodeId:{nodeId}");
        }

        CharacterSaveData newSaveData = new CharacterSaveData
        {
            CharacterId = charId,
            CurrentNodeId = nodeId,
            CurrentStats = startNode.StartStats.Clone()
        };

        _characterRepo.InitializeData(newSaveData);

        Debug.Log($"{_logClass} CharacterData Initialize");
    }

}