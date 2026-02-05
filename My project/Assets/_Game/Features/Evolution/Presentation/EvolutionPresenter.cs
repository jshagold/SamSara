using System;
using System.Collections.Generic;
using UnityEngine;

public class EvolutionPresenter : IDisposable
{
    private readonly string _logClass = $"{nameof(EvolutionPresenter)}";

    // Views
    private readonly EvolutionView _evolutionView;
    private readonly EvolutionPopupView _evolutionPopupView;

    // UseCases
    private readonly GetCharacterDetailUseCase _getCharacterDetailUseCase;
    private readonly GetEvolutionTreeUseCase _getTreeUseCase;

    // Resource Provider
    private readonly ICharacterResourceProvider _resourceProvider;

    // 멤버 변수 캐시 (라인 그리기 + 클릭 이벤트용 공용)
    private Dictionary<int, EvolutionNodeInfo> _nodeDataCache = new Dictionary<int, EvolutionNodeInfo>();

    public EvolutionPresenter(
        EvolutionView evolutionView,
        EvolutionPopupView evolutionPopupView,
        GetCharacterDetailUseCase getCharacterDetailUseCase,
        GetEvolutionTreeUseCase getEvolutionTreeUseCase,
        ICharacterResourceProvider resourceProvider)
    {
        _evolutionView = evolutionView;
        _evolutionPopupView = evolutionPopupView;
        _getCharacterDetailUseCase = getCharacterDetailUseCase;
        _getTreeUseCase = getEvolutionTreeUseCase;
        _resourceProvider = resourceProvider;
    }

    public void Initialize()
    {
        // 1. 이벤트 바인딩
        _evolutionView.OnClickBackButton(OnClickBack);

        // 2. 초기 화면 갱신
        Refresh();

        // 3. 스크롤 위치 초기화 (중앙)
        _evolutionView.ResetScrollPosition();
    }


    private void Refresh()
    {
        CharacterInfo characterInfo = _getCharacterDetailUseCase.Execute().Info;
        List<EvolutionNodeInfo> nodeInfoList = _getTreeUseCase.Execute();

        _evolutionView.ClearAll();
        _nodeDataCache.Clear();

        // 맵 크기 계산용 변수
        float maxAbsX = 0f;
        float maxAbsY = 0f;

        // 레벨별 Y좌표 범위 계산 Key: EvolutionLevel, Value: Vector2(Min Y, Max Y)
        Dictionary<int, Vector2> levelYBounds = new Dictionary<int, Vector2>();

        foreach(var nodeInfo in nodeInfoList)
        {
            // Dictionary에 노드등록
            _nodeDataCache[nodeInfo.Id] = nodeInfo;

            // 전체 맵의 최대 크기 갱신
            if(Mathf.Abs(nodeInfo.Position.x) > maxAbsX) maxAbsX = Mathf.Abs(nodeInfo.Position.x);
            if(Mathf.Abs(nodeInfo.Position.y) > maxAbsY) maxAbsY = Mathf.Abs(nodeInfo.Position.y);

            if(!levelYBounds.ContainsKey(nodeInfo.EvolutionLevel))
            {
                levelYBounds[nodeInfo.EvolutionLevel] = new Vector2(nodeInfo.Position.y, nodeInfo.Position.y);
            }
            else
            {
                var bounds = levelYBounds[nodeInfo.EvolutionLevel];
                bounds.x = Mathf.Min(bounds.x, nodeInfo.Position.y);    // Min Y
                bounds.y = Mathf.Max(bounds.y, nodeInfo.Position.y);    // Max Y
                levelYBounds[nodeInfo.EvolutionLevel] = bounds;
            }
        }

        // 스크롤 영역(Content Size) 설정
        float totalWidth = (maxAbsX * 2) + _evolutionView.MapPadding.x;
        float totalHeight = (maxAbsY * 2) + _evolutionView.MapPadding.y;

        // Background 그리기
        foreach(var kvp in levelYBounds)
        {
            int level = kvp.Key;
            float minY = kvp.Value.x;
            float maxY = kvp.Value.y;

            // 배경 중심 Y좌표 및 높이 계산 (위아래로 nodePadding 정도 여유 공간 추가)
            float yCenter = (minY + maxY) / 2f;
            float bgHeight = (maxY - minY) + _evolutionView.NodePadding *2;

            _evolutionView.CreateBackground(
                level: level,
                yCenter: yCenter,
                height: bgHeight,
                contentWidth: totalWidth);
        }

        // Line 그리기
        foreach(var parentNode in nodeInfoList)
        {
            foreach(int childId in parentNode.NextNodeIds)
            {
                if(_nodeDataCache.ContainsKey(childId))
                {
                    EvolutionNodeInfo childNode = _nodeDataCache[childId];

                    var lineView = _evolutionView.CreateLine();

                    lineView.DrawLine(
                        startPos: parentNode.Position,
                        endPos: childNode.Position,
                        thickness: _evolutionView.LineThickness,
                        color: _evolutionView.LineColor);
                }
            }
        }

        // Node 그리기
        foreach (var nodeInfo in nodeInfoList)
        {
            var nodeView = _evolutionView.CreateNode();

            // 위치 설정 (Domain의 좌표 -> View의 좌표)
            nodeView.SetPosition(nodeInfo.Position);

            Sprite icon = _resourceProvider.GetPortrait(characterId: characterInfo.Id, evolutionNodeId: nodeInfo.Id);

            nodeView.SetData(stateType: nodeInfo.State, icon: icon);

            nodeView.SetOnClick(() => OnClickNode(nodeInfo.Id));
        }

        _evolutionView.SetContentSize(new Vector2(totalWidth, totalHeight));
    }

    private void OnClickNode(int nodeId)
    {
        Debug.Log($"[EvolutionPresenter] Node Clicked: {nodeId}");
        if (_nodeDataCache.ContainsKey(nodeId))
        {
            EvolutionNodeInfo nodeInfo = _nodeDataCache[nodeId];

            // 캐릭터의 현재 스탯 정보 가져오기 (비교용)
            CharacterInfo charInfo = _getCharacterDetailUseCase.Execute().Info;

            // 진화 가능 여부 및 상태 체크
            bool isUnlocked = nodeInfo.State == EvolutionStateType.Possible;
            bool canEvolve = !isUnlocked && CheckEvolutionConditions(charInfo, nodeInfo);
            // TODO 텍스트 하드코딩 수정
            string requireLabel = "요구 조건";
            string evolveLabel = isUnlocked ? "완료" : "진화";
            string requirementsText = FormatRequirements(nodeInfo.StartStats);

            Debug.Log($"[{_logClass}] Open Popup for: {nodeInfo.Name}, CanEvolve: {canEvolve}");

            // 팝업 오픈
            _evolutionPopupView.OpenPopup(
                requireLabel: requireLabel,
                evolveLabel: evolveLabel,
                name: nodeInfo.Name,
                desc: nodeInfo.Desc,
                requirements: requirementsText,
                iconSprite: _resourceProvider.GetPortrait(charInfo.Id, nodeInfo.Id),
                canEvolve: canEvolve,
                onEvolveClick: () => OnEvolve(nodeInfo.Id)
            );
        }
    }

    private void OnEvolve(int nodeId)
    {
        Debug.Log($"[{_logClass}] Evolve Request: {nodeId}");

        // TODO: 1. 진화 UseCase 실행 (재화 소모, 스탯 반영 등)
        // TODO: 2. 성공 시 Refresh() 호출하여 트리 및 팝업 상태 갱신
        // _evolveUseCase.Execute(nodeId); 
        // Refresh();

        _evolutionPopupView.ClosePopup();
    }

    /// <summary>
    /// 진화 조건(요구 스탯) 달성 여부 체크
    /// </summary>
    private bool CheckEvolutionConditions(CharacterInfo charInfo, EvolutionNodeInfo nodeInfo)
    {
        // TODO 이전 단계(부모)가 잠겨있으면 진화 불가 로직 등이 여기에 추가될 수 있음.
        if (nodeInfo.State == EvolutionStateType.Locked) return false;

        StatGroup currentStats = charInfo.CurrentStats;
        StatGroup reqStats = nodeInfo.StartStats;

        bool isConditionMet = (currentStats.Hp.Value >= reqStats.Hp.Value) 
            && (currentStats.Strength.Value >= reqStats.Strength.Value)
            && (currentStats.Toughness.Value >= reqStats.Toughness.Value)
            && (currentStats.Agility.Value >= reqStats.Agility.Value);

        return isConditionMet;
    }

    /// <summary>
    /// 요구 스탯 정보를 UI 표시용 문자열로 변환
    /// </summary>
    private string FormatRequirements(StatGroup reqStats)
    {
        // TODO 텍스트 하드코딩 수정
        // 값이 0보다 큰 경우에만 표시하도록 필터링 가능
        List<string> reqs = new List<string>
        {
            "Require Stat: ",
            $"HP {reqStats.Hp}",
            $"Strength {reqStats.Strength}",
            $"Toughness {reqStats.Toughness}",
            $"Agility {reqStats.Agility}"
        };

        return string.Join("\n", reqs);
    }

    private void OnClickBack()
    {
        Debug.Log("[EvolutionPresenter] Back Button Clicked");
        UnityEngine.SceneManagement.SceneManager.LoadScene("CharacterInfoScene");
    }

    public void Dispose()
    {

    }
}