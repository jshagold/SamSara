namespace Core.System
{
    /// <summary>
    /// 앱 종료(에디터: 플레이 중지 / 빌드: Quit) 책임. 단일 책임·DI용.
    /// </summary>
    public interface IAppLifecycleService
    {
        void Quit();
    }
}
