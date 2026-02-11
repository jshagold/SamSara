using UnityEngine;

public class IntroBootstrapper : MonoBehaviour
{
    [SerializeField] private IntroView _introView;

    private IntroPresenter _introPresenter;

    private void Start()
    {
        if (GlobalBootstrapper.Instance == null)
        {
            Debug.LogError("GlobalBootstrapper 선언되지 않음");
            return;
        }
        var gameContext = GlobalBootstrapper.Instance.GameContext;

        _introPresenter = new IntroPresenter(
            introView: _introView,
            popupManager: gameContext.PopupManager);

        _introPresenter.Initialize();
    }

    private void OnDestroy()
    {
        _introPresenter?.Dispose();
    }
}