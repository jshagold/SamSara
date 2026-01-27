using System;
using System.Collections.Generic;

public class GetEvolutionTreeUseCase
{
    private readonly string _logClass = $"{nameof(GetEvolutionTreeUseCase)}";

    private readonly ICharacterRepository _characterRepo;
    private readonly ICharacterMasterRepository _masterRepo;

    public GetEvolutionTreeUseCase(
        ICharacterRepository characterRepository,
        ICharacterMasterRepository characterMasterRepository) 
    {
        _characterRepo = characterRepository;
        _masterRepo = characterMasterRepository;
    }

    public List<EvolutionNodeInfo> Execute()
    {
        if(!_characterRepo.HasSaveData()) throw new InvalidOperationException($"{_logClass} Character 저장 데이터 파일 존재하지않음");

        var saveData = _characterRepo.GetCharacterData();
        var masterData = _masterRepo.GetData(saveData.CharacterId);

        Dictionary<int, int> parentMap = BuildParentMap(masterData.EvolutionNodes);
        // 조상 노드 추적 (Completed 상태 판별용)
        HashSet<int> ancestorIds = FindAncestorPath(
            currentNodeId: saveData.CurrentNodeId,
            parentMap: parentMap,
            rootId: masterData.RootNodeId
        );

        var resultList = new List<EvolutionNodeInfo>();

        foreach(var node in masterData.EvolutionNodes)
        {
            var nodeInfo = node.ToDomain();
            nodeInfo.State = CalculateState(
                nodeData: node, 
                saveData: saveData,
                ancestorIds: ancestorIds,
                parentMap: parentMap);

            resultList.Add(nodeInfo);
        }

        return resultList;
    }

    private Dictionary<int, int> BuildParentMap(List<EvolutionNodeData> allNodes)
    {
        var map = new Dictionary<int, int>();
        foreach(var parentNode in allNodes)
        {
            foreach(var childId in parentNode.NextNodeIds)
            {
                if(!map.ContainsKey(childId))
                {
                    map.Add(childId, parentNode.Id);
                }
            }
        }

        return map;
    }

    private HashSet<int> FindAncestorPath(int currentNodeId, Dictionary<int, int> parentMap, int rootId)
    {
        var ancestors = new HashSet<int>();
        int checkId = currentNodeId;

        if(checkId == rootId) return ancestors;

        // Root에 닿을때까지 부모타고 올라감
        while(parentMap.ContainsKey(checkId))
        {
            int parentId = parentMap[checkId];
            ancestors.Add(parentId);

            checkId = parentId;

            if(checkId == rootId) break;
        }

        return ancestors;
    }

    private EvolutionStateType CalculateState(
        EvolutionNodeData nodeData, 
        CharacterSaveData saveData,
        HashSet<int> ancestorIds,
        Dictionary<int, int> parentMap)
    {
        if(saveData.CurrentNodeId == nodeData.Id)
        {
            return EvolutionStateType.Current;
        }

        if(ancestorIds.Contains(nodeData.Id))
        {
            return EvolutionStateType.Completed;
        }

        bool isParentCurrent = false;
        if(parentMap.TryGetValue(nodeData.Id, out int myParentId))
        {
            if(myParentId == saveData.CurrentNodeId) isParentCurrent = true;
        }

        if (isParentCurrent)
        {
            if (CheckStatCondition(current: saveData.CurrentStats, required: nodeData.StartStats))
            {
                return EvolutionStateType.Possible;
            }
            else
            {
                return EvolutionStateType.Impossible;
            }
        }
        
        return EvolutionStateType.Locked;
    }

    private bool CheckStatCondition(StatGroup current, StatGroup required)
    {
        bool hpOk = current.Hp.Value >= required.Hp.Value;
        bool strOk = current.Strength.Value >= required.Strength.Value;
        bool tghOk = current.Toughness.Value >= required.Toughness.Value;
        bool agiOk = current.Agility.Value >= required.Agility.Value;

        return hpOk && strOk && tghOk && agiOk;
    }
}