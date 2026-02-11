using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterMasterData", menuName = "Samsara/Character Master Data")]
public class CharacterMasterData : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private int _id;
    [SerializeField] private string _name;
    [SerializeField] [TextArea] private string _desc;

    [Header("Evolution Graph")]
    [SerializeField] private List<EvolutionNodeData> _evolutionNodes;
    [SerializeField] private int _rootNodeId;

    public int Id => _id;
    public string Name => _name;
    public string Desc => _desc;
    public List<EvolutionNodeData> EvolutionNodes => _evolutionNodes;
    public int RootNodeId => _rootNodeId;
}