using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterMasterData", menuName = "Samsara/Character Master Data")]
public class CharacterMasterData : ScriptableObject
{
    [Header("Identity")]
    public int CharacterId;
    public string CharacterName;
    [TextArea] public string Desc;

    [Header("Evolution Graph")]
    public List<CharacterEvolutionNode> EvolutionNodes;
    public int RootNodeId;

    public CharacterEvolutionNode GetNode(int nodeId)
    {
        return EvolutionNodes.FirstOrDefault(n => n.nodeId == nodeId);
    }

}