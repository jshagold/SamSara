using System.Threading;
using Cysharp.Threading.Tasks;

namespace Samsara.Features.ReplayScene.Domain
{
    public interface IRestartFlow
    {
        UniTask ExecuteAsync(string selectedNodeId, PendingReplayContext pendingContext, CancellationToken ct = default);
    }
}
