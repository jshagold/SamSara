using Cysharp.Threading.Tasks;
using Samsara.Features.Character.Data;

namespace Samsara.Features.Character.Domain
{
    public interface ICharacterAccountRepository
    {
        CharacterAccountData AccountData { get; }
        void UnlockEvolutionNode(string nodeId);
        void RegisterCodex(string nodeId);
        UniTask SaveDataAsync();
        void SaveDataSync();
        UniTask LoadDataAsync();
    }
}
