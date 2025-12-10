using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

public class IntroPresenter
{
    private IntroView _introView;
    private string _nextSceneName;

    public IntroPresenter(IntroView introView)
    {
        _introView = introView;
    }

    public async void Initialize()
    {
        // 1. 초기화면 세팅 (로고 on, 타이틀 off)
        _introView.SetupInitialState();

        // 2. 데이터 로딩 (최소 2초 보장)
        var loadTask = GlobalBootstrapper.Instance.LoadAllGameDataAsync();
        var waitTask = UniTask.Delay(2000); // 로고 재생 시간

        await UniTask.WhenAll(loadTask, waitTask);

        bool isRestartGame = CheckStartOption();

        if (isRestartGame)
        {
            // 재시작
            // TODO
            _nextSceneName = "RestartScene";
        }
        else
        {
            // 일반시작
            // TODO 튜토리얼 넣을지 정해야함.
            _nextSceneName = "MainScene";
        }

        await _introView.TransitionToTitle(isRestart: isRestartGame);

        _introView.SetOnClickStartButton(OnStartButtonClicked);
    }

    private void OnStartButtonClicked()
    {
        SceneManager.LoadScene( _nextSceneName );
    }

    private bool CheckStartOption()
    {
        // TODO Repository에서 값 받아와야함

        return false;
    }
}