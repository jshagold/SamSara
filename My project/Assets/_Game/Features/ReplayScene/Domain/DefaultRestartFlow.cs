using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Samsara.Features.ReplayScene.Domain
{
    public class DefaultRestartFlow : IRestartFlow
    {
        private readonly string _logClass = $"[{nameof(DefaultRestartFlow)}]";

        private readonly GameContext _gameContext;

        public DefaultRestartFlow(GameContext gameContext)
        {
            _gameContext = gameContext;
        }

        public async UniTask ExecuteAsync(string selectedNodeId, PendingReplayContext pendingContext, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(selectedNodeId))
                throw new InvalidOperationException($"{_logClass} ExecuteAsync: selectedNodeId is null or empty");

            // Fail Fast — string → int 변환 실패는 자연 throw (Constitution §7).
            int intNodeId = int.Parse(selectedNodeId);

            await _gameContext.ResetRunForReplayAsync(intNodeId);
        }
    }
}
