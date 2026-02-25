using Cysharp.Threading.Tasks;

namespace Core.ErrorHandling
{
    /// <summary>
    /// Wrapper holding the exact failed operation for 1:1 retry (FR-06). No side effects.
    /// </summary>
    public class NetworkRetryCommand
    {
        private readonly System.Func<UniTask> _retryAction;

        public NetworkRetryCommand(System.Func<UniTask> retryAction)
        {
            _retryAction = retryAction;
        }

        public UniTask Invoke() => _retryAction();
    }
}
