using Cysharp.Threading.Tasks;
using System;

namespace Core.ErrorHandling
{
    /// <summary>
    /// Wrapper holding the exact failed operation for 1:1 retry (FR-06). No side effects.
    /// </summary>
    public class NetworkRetryCommand
    {
        private readonly Func<UniTask> _retryAction;

        public NetworkRetryCommand(Func<UniTask> retryAction)
        {
            _retryAction = retryAction;
        }

        public UniTask Invoke() => _retryAction();
    }
}
