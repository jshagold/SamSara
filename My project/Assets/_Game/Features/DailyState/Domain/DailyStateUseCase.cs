using System;

public class DailyStateUseCase
{
    private readonly IDailyStateRepository _gameStateRepository;

    public event Action OnCharacterUpdated
    {
        add => _gameStateRepository.OnDailyStateChanged += value;
        remove => _gameStateRepository.OnDailyStateChanged -= value;
    }

    public DailyStateUseCase(IDailyStateRepository gameStateRepository)
    {
        _gameStateRepository = gameStateRepository;
    }
    
    public int GetCurrentDay()
    {
        return _gameStateRepository.GetCurrentDay();
    }

    public bool[] GetActionSlot(int charId)
    {
        return _gameStateRepository.GetActionSlot(charId: charId);
    }
}