using System.Threading;
using Cysharp.Threading.Tasks;

namespace Samsara.Core.Navigation
{
    public interface ISceneNavigator
    {
        UniTask NavigateToAsync(SceneKey key);
        UniTask NavigateToAsync(SceneKey key, CancellationToken ct);
    }
}
