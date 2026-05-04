using Cysharp.Threading.Tasks;
using Samsara.Features.Character.Data;

namespace Samsara.Features.Character.Domain
{
    public interface ICharacterRunRepository
    {
        CharacterRunData RunData { get; }
        void InitializeNewRun(RunConfigSO config, int? overrideEvolutionNodeId = null);
        void MarkDirty();
        UniTask SaveDataAsync();
        void SaveDataSync();
        UniTask LoadDataAsync();
    }
}
