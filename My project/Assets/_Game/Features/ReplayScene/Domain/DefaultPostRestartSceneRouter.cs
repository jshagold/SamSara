using Samsara.Core.Navigation;

namespace Samsara.Features.ReplayScene.Domain
{
    public class DefaultPostRestartSceneRouter : IPostRestartSceneRouter
    {
        private readonly string _logClass = $"[{nameof(DefaultPostRestartSceneRouter)}]";

        public SceneKey ResolveNextSceneKey(string selectedNodeId, PendingReplayContext pendingContext)
        {
            return SceneKey.Main;
        }
    }
}
