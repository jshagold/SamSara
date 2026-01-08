using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterMasterData", menuName = "Samsara/Character Master Data")]
public class CharacterMasterData : ScriptableObject
{
    [Header("Identity")]
    public int Id;
    public string Name;
    [TextArea] public string Desc;

    [Header("Evolution Graph")]
    public List<EvolutionNodeData> EvolutionNodes;
    public int RootNodeId;
}