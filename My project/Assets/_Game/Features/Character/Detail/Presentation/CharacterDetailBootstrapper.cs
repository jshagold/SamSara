using UnityEngine;

public class CharacterDetailBootstrapper: MonoBehaviour
{
    private readonly string _logClass = $"[{nameof(CharacterDetailBootstrapper)}]";

    [SerializeField] private CharacterDetailView _characterDetailView;

    // Presenters
    private CharacterDetailPresenter _characterDetailPresenter;
    private SkillListPresenter _skillListPresenter;
    private InventoryPresenter _inventoryPresenter;

    // ResourceProvider
    private ICharacterResourceProvider _characterResourceProvider;
    private ISkillResourceProvider _skillResourceProvider;
    private IItemResourceProvider _itemResourceProvider;

    public void Initialize(GameContext gameContext)
    {
        // UseCases
        var getCharacterDetailUseCase = gameContext.GetCharacterDetailUseCase;
        var getSkillListUseCase = gameContext.GetSkillListUseCase;
        var getInventoryUseCase = gameContext.GetInventoryUseCase;

        // ResourceProviders
        var masterDataManager = gameContext.MasterDataManager;
        _characterResourceProvider = new CharacterResourceProvider(masterRepo: masterDataManager.CharacterRepo);
        _skillResourceProvider = new SkillResourceProvider(skillMasterRepo: masterDataManager.SkillRepo);
        _itemResourceProvider = new ItemResourceProvider(masterRepo: masterDataManager.ItemRepo);

        // Presenters
        _characterDetailPresenter = new CharacterDetailPresenter(
            characterDetailView: _characterDetailView,
            getCharacterDetailUseCase: getCharacterDetailUseCase,
            resourceProvider: _characterResourceProvider);
        _characterDetailPresenter.Initialize();

        _skillListPresenter = new SkillListPresenter(
            skillListView: _characterDetailView.SkillListView,
            getSkillListUseCase: getSkillListUseCase,
            skillResourceProvider: _skillResourceProvider);
        _skillListPresenter.Initialize();

        _inventoryPresenter = new InventoryPresenter(
            inventoryView: _characterDetailView.InventoryView,
            getInventoryUseCase: getInventoryUseCase,
            itemResourceProvider: _itemResourceProvider);
        _inventoryPresenter.Initialize();

        Debug.Log($"{_logClass} Initialize");
    }

    private void OnDestroy()
    {
        _characterDetailPresenter.Dispose();
        _characterDetailPresenter = null;

        _skillListPresenter.Dispose();
        _skillListPresenter = null;

        _inventoryPresenter?.Dispose();
        _inventoryPresenter = null;

        _characterResourceProvider = null;
        _skillResourceProvider = null;
        _itemResourceProvider = null;
    }
}