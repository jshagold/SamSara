using UnityEngine;

public class CharacterDetailBootstrapper: MonoBehaviour
{
    private readonly string _logClass = $"[{nameof(CharacterDetailBootstrapper)}]";

    [SerializeField] private CharacterDetailView _characterDetailView;

    // Presenters
    private CharacterDetailPresenter _characterDetailPresenter;

    public void Initialize(
        GameContext gameContext)
    {
        Debug.Log($"{_logClass} Initialize");
        _characterDetailPresenter = new CharacterDetailPresenter(
            characterDetailView: _characterDetailView,
            getCharacterDetailUseCase: ,
            characterMasterRepository:);
    }

    private void OnDestroy()
    {
        
    }
}