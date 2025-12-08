using System.Diagnostics;
using Cysharp.Threading.Tasks;

public class GameStateUseCase
{
    private readonly IGameStateRepository _gameStateRepository;

    public GameStateUseCase(IGameStateRepository gameStateRepository)
    {
        _gameStateRepository = gameStateRepository;
    }

    // charId의 행동횟수 1 소모
    public void ExecuteAction(int charId)
    {
        var actionSlot = _gameStateRepository.GetActionSlot(charId);
        var consumeIndex = -1;

        for (var slotIndex = 0; slotIndex < actionSlot.Length; slotIndex++)
        {
            if (!actionSlot[slotIndex]) break;
            else consumeIndex = slotIndex;
        }

        if(consumeIndex != -1 && consumeIndex >= actionSlot.Length) {
            _gameStateRepository.ConsumeActionSlot(charId: charId, slotIndex: 0);
        }
    }


    // 게임 저장
    public async UniTask SaveGame()
    {
        await _gameStateRepository.SaveDataAsync();
    }
}