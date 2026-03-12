using UnityEngine.SceneManagement;

/// <summary>
/// ISceneNavigator 구체 구현. 씬 전환의 단일 진입점.
/// </summary>
public class SceneNavigator : ISceneNavigator
{
    private readonly string _logClass = $"[{nameof(SceneNavigator)}]";

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
