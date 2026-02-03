using System;
using System.Collections.Generic;
using UnityEngine;

public class EvolutionPresenter : IDisposable
{
    private readonly string _logClass = $"{nameof(EvolutionPresenter)}";

    // Views
    private readonly EvolutionView _evolutionView;

    // UseCases
    private readonly GetCharacterDetailUseCase _getCharacterDetailUseCase;
    private readonly GetEvolutionTreeUseCase _getTreeUseCase;

    // Resource Provider
    private readonly ICharacterResourceProvider _resourceProvider;

    public EvolutionPresenter(
        EvolutionView evolutionView,
        GetCharacterDetailUseCase getCharacterDetailUseCase,
        GetEvolutionTreeUseCase getEvolutionTreeUseCase,
        ICharacterResourceProvider resourceProvider)
    {
        _evolutionView = evolutionView;
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

        // 맵 크기 계산용 변수
        float maxAbsX = 0f;
        float maxAbsY = 0f;

        // 레벨별 Y좌표 범위 계산 Key: EvolutionLevel, Value: Vector2(Min Y, Max Y)
        Dictionary<int, Vector2> levelYBounds = new Dictionary<int, Vector2>();

        // 라인 그리기를 위한 부모 노드 빠른 검색용 딕셔너리
        Dictionary<int, EvolutionNodeInfo> nodeDict = new Dictionary<int, EvolutionNodeInfo>();

        foreach(var nodeInfo in nodeInfoList)
        {
            // Dictionary에 노드등록
            nodeDict[nodeInfo.Id] = nodeInfo;

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
                if(nodeDict.ContainsKey(childId))
                {
                    EvolutionNodeInfo childNode = nodeDict[childId];

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
        // TODO: 상세 팝업 오픈
    }

    private void OnClickBack()
    {
        // TODO: 씬 이동 처리
        Debug.Log("[EvolutionPresenter] Back Button Clicked");
    }

    public void Dispose()
    {

    }
}