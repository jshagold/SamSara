using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class IntroView : MonoBehaviour
{
    [Header("스플래시/타이틀 그룹")]
    [SerializeField] private CanvasGroup splashGroup;
    [SerializeField] private CanvasGroup titleGroup;

    [Header("Title Elements")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Button startButton;
    [SerializeField] private TextMeshProUGUI buttonText;

    [Header("Resources (일반/재시작 배경)")]
    [SerializeField] private Sprite backgroundNormal;
    [SerializeField] private Sprite backgroundSamsara;

    // 초기 상태 설정 - 로고만 보이고 타이틀 숨김
    public void SetupInitialState()
    {
        splashGroup.alpha = 1f;
        titleGroup.alpha = 0f;
        titleGroup.interactable = false; // 클릭 방지
    }

    // 로고 서서히 사라지고 타이틀 나타나는 효과 적용(Cross Fade)
    public async UniTask TransitionToTitle(bool isRestart)
    {
        // 토큰 가져오기 (UniTask가 제공하는 강력한 기능)
        // 이 MonoBehaviour가 Destroy 되면 토큰도 Cancel 상태가 된다.
        var cancellationToken = this.GetCancellationTokenOnDestroy();

        if (isRestart)
            ShowSamsaraMode();
        else
            ShowNormalMode();

        // Fade 애니메이션 (0.5초)
        float duration = 0.5f;
        float elasped = 0f;

        while (elasped < duration)
        {
            elasped += Time.deltaTime;
            float t = elasped / duration;

            splashGroup.alpha = 1 - t;
            titleGroup.alpha = t;

            // Yield에 토큰 전달 [핵심]
            // 만약 도중에 객체가 파괴되면 여기서 OperationCanceledException이 발생하며 
            // 아래 코드를 실행하지 않고 조용히 종료됩니다.
            await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
        }
    }

    // 게임시작 버튼 이벤트 연결
    public void SetOnClickStartButton(UnityAction action)
    {
        startButton.onClick.RemoveAllListeners();
        startButton.onClick.AddListener(action);
    }

    // 일반 시작화면 설정
    public void ShowNormalMode()
    {
        backgroundImage.sprite = backgroundNormal;
        string text = LocalizationUtils.GetString(key: "scene_intro_start_button_normal");
        buttonText.text = text;
    }

    // 재시작 화면 설정
    public void ShowSamsaraMode()
    {
        backgroundImage.sprite = backgroundSamsara;
        string text = LocalizationUtils.GetString(key: "scene_intro_start_button_samsara");
        buttonText.text = text;
    }
}