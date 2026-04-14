using Cysharp.Threading.Tasks;
using Samsara.Features.Character.Data;

namespace Samsara.Features.Character.Domain
{
    public interface ICharacterRunRepository
    {
        CharacterRunData RunData { get; }
        void InitializeNewRun(RunConfigSO config);
        void MarkDirty();
        UniTask SaveDataAsync();
        void SaveDataSync();
        UniTask LoadDataAsync();
    }
}
