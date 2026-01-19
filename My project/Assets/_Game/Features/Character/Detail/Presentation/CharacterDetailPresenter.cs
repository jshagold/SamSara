using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterDetailPresenter : IDisposable
{
    private readonly string _logClass = $"{nameof(CharacterDetailPresenter)}";

    // Views
    private readonly CharacterDetailView _characterDetailView;

    // UseCases
    private readonly GetCharacterDetailUseCase _getCharacterDetailUC;

    // Resource Provider
    private readonly ICharacterResourceProvider _resourceProvider;

    // Sub Presenters
    private StatListPresenter _statListPresenter;

    public CharacterDetailPresenter(
        CharacterDetailView characterDetailView,
        GetCharacterDetailUseCase getCharacterDetailUseCase,
        ICharacterResourceProvider resourceProvider) 
    {
        _characterDetailView = characterDetailView;
        _getCharacterDetailUC = getCharacterDetailUseCase;
        _resourceProvider = resourceProvider;
    }

    public void Initialize()
    {
        // 이벤트 binding
        _characterDetailView.OnClickBackButton(ToMainScene);
        _characterDetailView.OnClickEvoSceneButton(ToEvolutionScene);

        // Sub Presenters
        _statListPresenter = new StatListPresenter(statListView: _characterDetailView.StatListView);

        // Logic Event
        _getCharacterDetailUC.OnCharacterUpdated += Refresh;

        Refresh();
    }

    private void Refresh()
    {
        var characterTuple = _getCharacterDetailUC.Execute();
        CharacterInfo characterInfo = characterTuple.Info;
        List<StatDisplayInfo> statList = characterTuple.Stats;

        Sprite portrait = _resourceProvider.GetPortrait(characterId: characterInfo.Id, evolutionNodeId: characterInfo.CurrentEvolutionNode.Id);

        // TODO CharacterName Localization 해야함
        _characterDetailView.SetTitleText(text: characterInfo.Name);
        _characterDetailView.SetCharacterName(name: characterInfo.Name);
        _characterDetailView.SetCharacterImage(sprite: portrait);

        // Sub Presenters
        _statListPresenter.Refresh(statList: statList);
    }

    // 정비 씬으로 이동
    private void ToEvolutionScene()
    {
        // TODO 진화단계 씬 이름 정의해야함.
        UnityEngine.SceneManagement.SceneManager.LoadScene("EvolutionScene");
    }

    // 메인 씬으로 이동
    private void ToMainScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
    }

    public void Dispose()
    {
        if(_getCharacterDetailUC != null)
        {
            _getCharacterDetailUC.OnCharacterUpdated -= Refresh;
        }
    }
}