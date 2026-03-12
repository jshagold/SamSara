using UnityEngine;

public class CharacterResourceProvider : ICharacterResourceProvider
{
    private readonly string _logClass = $"[{nameof(CharacterResourceProvider)}]";
    private readonly ICharacterMasterRepository _masterRepo;

    public CharacterResourceProvider(ICharacterMasterRepository masterRepo)
    {
        _masterRepo = masterRepo ?? throw new System.ArgumentNullException(nameof(masterRepo));
    }

    public Sprite GetPortrait(int characterId, int evolutionNodeId)
    {
        var masterData = _masterRepo.GetData(characterId);
        if (masterData == null)
            throw new System.InvalidOperationException($"{_logClass} MasterData not found charId:{characterId}");

        var nodeData = masterData.EvolutionNodes?.Find(node => node.Id == evolutionNodeId);
        if (nodeData == null)
            throw new System.InvalidOperationException($"{_logClass} EvolutionNode not found nodeId:{evolutionNodeId}");

        return nodeData.Portrait;
    }
}