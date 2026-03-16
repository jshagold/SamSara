using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using Samsara.Core.Navigation;

namespace Samsara.App
{
    public class SceneNavigator : ISceneNavigator
    {
        private readonly string _logClass = $"[{nameof(SceneNavigator)}]";

        public UniTask NavigateToAsync(SceneKey key)
            => NavigateToAsync(key, CancellationToken.None);

        public async UniTask NavigateToAsync(SceneKey key, CancellationToken ct)
        {
            var sceneName = key.ToString();

            if (SceneManager.GetActiveScene().name == sceneName)
            {
                Debug.Log($"{_logClass} Already at: {key}, skipping");
                return;
            }

            Debug.Log($"{_logClass} Navigating to: {key}");
            await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single)
                .ToUniTask(cancellationToken: ct);
            Debug.Log($"{_logClass} Arrived: {key}");
        }
    }
}
