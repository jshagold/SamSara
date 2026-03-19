using Cysharp.Threading.Tasks;
using Samsara.Features.Character.Data;
using Samsara.Features.Character.MasterData;

namespace Samsara.Features.Character.Domain
{
    public interface ICharacterRunRepository
    {
        CharacterRunData RunData { get; }
        void InitializeNewRun(string startingEvolutionNodeId, CharacterStatsSO baseStats);
        UniTask SaveDataAsync();
        void SaveDataSync();
        UniTask LoadDataAsync();
    }
}
