using System;
using System.Collections.Generic;
using UnityEngine;

public class CharacterDetailPresenter : IDisposable
{
    private readonly string _logClass = $"[{nameof(CharacterDetailPresenter)}]";

    // Views
    private readonly CharacterDetailView _characterDetailView;

    // UseCases
    private readonly GetCharacterDetailUseCase _getCharacterDetailUC;

    // Resource Provider
    private readonly ICharacterResourceProvider _resourceProvider;

    private readonly StatListPresenter _statListPresenter;

    public CharacterDetailPresenter(
        CharacterDetailView characterDetailView,
        GetCharacterDetailUseCase getCharacterDetailUseCase,
        ICharacterResourceProvider resourceProvider,
        StatListPresenter statListPresenter)
    {
        _characterDetailView = characterDetailView ?? throw new ArgumentNullException(nameof(characterDetailView));
        _getCharacterDetailUC = getCharacterDetailUseCase ?? throw new ArgumentNullException(nameof(getCharacterDetailUseCase));
        _resourceProvider = resourceProvider ?? throw new ArgumentNullException(nameof(resourceProvider));
        _statListPresenter = statListPresenter ?? throw new ArgumentNullException(nameof(statListPresenter));
    }

    public void Initialize()
    {
        _characterDetailView.OnClickBackButton(ToMainScene);
        _characterDetailView.OnClickEvoSceneButton(ToEvolutionScene);

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
        _getCharacterDetailUC.OnCharacterUpdated -= Refresh;
    }
}