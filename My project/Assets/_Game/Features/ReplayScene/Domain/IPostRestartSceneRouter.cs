using Samsara.Core.Navigation;

namespace Samsara.Features.ReplayScene.Domain
{
    public interface IPostRestartSceneRouter
    {
        SceneKey ResolveNextSceneKey(string selectedNodeId, PendingReplayContext pendingContext);
    }
}
