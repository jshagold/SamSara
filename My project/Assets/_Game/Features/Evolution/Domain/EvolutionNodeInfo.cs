using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EvolutionNodeInfo
{
    public int Id;
    public string Name;
    public string Desc;
    public string PortraitName;

    public int EvolutionLevel;

    public StatGroup StartStats = new StatGroup();  // 진화 조건
    public StatGroup MaxStats = new StatGroup();    // 노드의 최대 스탯

    public List<SkillInfo> SkillList = new List<SkillInfo>();

    public List<int> NextNodeIds = new List<int>();
    public Vector2 Position;            // UI 배치 좌표
    public EvolutionStateType State;    // 현재 노드 해금상태

    public EvolutionNodeInfo() { }
}