using UnityEngine;

public interface ICharacterResourceProvider
{
    Sprite GetPortrait(int characterId, int evolutionNodeId); 
}