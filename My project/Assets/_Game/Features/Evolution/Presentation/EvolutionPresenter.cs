using System;
using System.Collections.Generic;
using UnityEngine;

public class EvolutionPresenter : IDisposable
{
    // Views
    private readonly EvolutionView _evolutionView;

    // UseCases
    private readonly GetCharacterDetailUseCase _getCharacterDetailUseCase;
    private readonly GetEvolutionTreeUseCase _getTreeUseCase;

    // Resource Provider
    private readonly ICharacterResourceProvider _resourceProvider;

    // Settings
    private readonly Vector2 _padding = new Vector2(300f, 300f); // 노드 바깥 여백

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
        List<EvolutionNodeInfo> nodeInfoList = _getTreeUseCase.Execute();
        CharacterInfo characterInfo = _getCharacterDetailUseCase.Execute().Info;

        _evolutionView.ClearNodeList();

        // 맵 크기 계산용 변수
        float maxAbsX = 0f;
        float maxAbsY = 0f;

        foreach(var nodeInfo in nodeInfoList)
        {
            var nodeView = _evolutionView.CreateNode();

            // 위치 설정 (Domain의 좌표 -> View의 좌표)
            nodeView.SetPosition(nodeInfo.Position);

            // 맵 크기 갱신 (가장 멀리 있는 노드 찾기)
            if (Mathf.Abs(nodeInfo.Position.x) > maxAbsX) maxAbsX = Mathf.Abs(nodeInfo.Position.x);
            if (Mathf.Abs(nodeInfo.Position.y) > maxAbsY) maxAbsY = Mathf.Abs(nodeInfo.Position.y);

            Sprite icon = _resourceProvider.GetPortrait(characterId: characterInfo.Id, evolutionNodeId: nodeInfo.Id);

            nodeView.SetData(stateType: nodeInfo.State, icon: icon);

            nodeView.SetOnClick(() => OnClickNode(nodeInfo.Id));
        }

        // 스크롤 영역(Content Size) 설정
        float width = (maxAbsX * 2) + _padding.x;
        float height = (maxAbsY * 2) + _padding.y;

        _evolutionView.SetContentSize(new Vector2(width, height));
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