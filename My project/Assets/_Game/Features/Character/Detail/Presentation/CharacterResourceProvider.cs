using UnityEngine;

public class CharacterResourceProvider : ICharacterResourceProvider
{
    private readonly string _logClass = $"{nameof(CharacterResourceProvider)}";
    private readonly ICharacterMasterRepository _masterRepo;

    public CharacterResourceProvider(ICharacterMasterRepository masterRepo)
    {
        _masterRepo = masterRepo;
    }

    public Sprite GetPortrait(int characterId, int evolutionNodeId)
    {
        var masterData = _masterRepo.GetData(characterId);
        if(masterData == null)
        {
            Debug.LogWarning($"{_logClass} MasterData not found charId:{characterId}");
            return null;
        }

        var nodeData = masterData.EvolutionNodes.Find(node => node.Id == evolutionNodeId);
        if (nodeData == null)
        {
            Debug.LogWarning($"{_logClass} EvolutionNode not found nodeId:{evolutionNodeId}");
            return null;
        }

        return nodeData.Portrait;
    }
}