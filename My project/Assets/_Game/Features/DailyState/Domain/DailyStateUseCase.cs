using System.Diagnostics;
using Cysharp.Threading.Tasks;

public class DailyStateUseCase
{
    private readonly IDailyStateRepository _gameStateRepository;

    public DailyStateUseCase(IDailyStateRepository gameStateRepository)
    {
        _gameStateRepository = gameStateRepository;
    }
    
    public int GetCurrentDay()
    {
        return _gameStateRepository.GetCurrentDay;
    }

    public bool[] GetActionSlot(string charId)
    {
        return _gameStateRepository.GetActionSlot(charId: charId);
    }
}