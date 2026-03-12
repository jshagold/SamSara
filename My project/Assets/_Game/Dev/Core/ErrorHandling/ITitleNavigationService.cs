namespace Core.ErrorHandling
{
    /// <summary>
    /// Title-screen navigation contract. Domain must not call SceneManager directly (FR-06).
    /// </summary>
    public interface ITitleNavigationService
    {
        void NavigateToTitle();
    }
}
