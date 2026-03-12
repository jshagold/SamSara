using Core.ErrorHandling;
using UnityEngine.SceneManagement;

namespace App.Systems.ErrorHandling
{
    /// <summary>
    /// Implements ITitleNavigationService; loads IntroScene (FR-06, Constitution C1/H1).
    /// </summary>
    public class TitleNavigationService : ITitleNavigationService
    {
        private const string TitleSceneName = "IntroScene";

        public void NavigateToTitle()
        {
            SceneManager.LoadScene(TitleSceneName);
        }
    }
}
