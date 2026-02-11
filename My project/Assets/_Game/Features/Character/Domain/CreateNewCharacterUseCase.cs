using System;
using UnityEngine;

public class CreateNewCharacterUseCase
{
    private readonly string _logClass = $"[{nameof(CreateNewCharacterUseCase)}]";

    private readonly ICharacterRepository _characterRepo;
    private readonly ICharacterMasterRepository _characterMasterRepo;
    private readonly NewGameConfig _newGameConfig;

    public CreateNewCharacterUseCase(
        ICharacterRepository characterRepo,
        ICharacterMasterRepository characterMasterRepo,
        NewGameConfig newGameConfig)
    {
        _characterRepo = characterRepo;
        _characterMasterRepo = characterMasterRepo;
        _newGameConfig = newGameConfig;
    }

    /// <summary>True when no character save exists and new game data should be created.</summary>
    public bool IsNewGameRequired() => !_characterRepo.HasSaveData();

    public void Execute()
    {
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