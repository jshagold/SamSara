using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterMasterData", menuName = "Samsara/Character Master Data")]
public class CharacterMasterData : ScriptableObject
{
    [Header("Identity")]
    public int id;
    public string characterName;
    [TextArea] public string desc;

    [Header("Evolution Graph")]
    public List<CharacterEvolutionNode> evolutionNodes;
    public int rootNodeId;

    public CharacterEvolutionNode GetNode(int nodeId)
    {
        return evolutionNodes.FirstOrDefault(n => n.nodeId == nodeId);
    }

}