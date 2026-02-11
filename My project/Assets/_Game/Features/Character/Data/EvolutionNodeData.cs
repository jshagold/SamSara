using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EvolutionNodeData
{
    [Header("Node Info")]
    [SerializeField] private int _id;
    [SerializeField] private string _name;
    [SerializeField] [TextArea] private string _desc;
    [SerializeField] private Sprite _portrait;

    [Header("Level Info")]
    [SerializeField] private int _level;

    [Header("Branching")]
    [SerializeField] private int _parentId;
    [SerializeField] private List<int> _nextNodeIds = new List<int>();

    [Header("Stats")]
    [SerializeField] private StatGroup _startStats = new StatGroup();
    [SerializeField] private StatGroup _maxStats = new StatGroup();

    [Header("Skills")]
    [SerializeField] private List<SkillMasterData> _skillList = new List<SkillMasterData>();

    [Header("UI Layout")]
    [SerializeField] private Vector2 _position;

    public int Id => _id;
    public string Name => _name;
    public string Desc => _desc;
    public Sprite Portrait => _portrait;
    public int Level => _level;
    public int ParentId => _parentId;
    public List<int> NextNodeIds => _nextNodeIds;
    public StatGroup StartStats => _startStats;
    public StatGroup MaxStats => _maxStats;
    public List<SkillMasterData> SkillList => _skillList;
    public Vector2 Position => _position;
}