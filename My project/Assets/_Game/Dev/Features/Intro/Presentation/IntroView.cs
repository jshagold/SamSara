using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class IntroView : MonoBehaviour
{
    [Header("스플래시/타이틀 그룹")]
    [SerializeField] private CanvasGroup _splashPanel;
    [SerializeField] private CanvasGroup _titlePanel;

    [Header("Title Elements")]
    [SerializeField] private Image _backgroundImage;
    [SerializeField] private Button _startButton;
    [SerializeField] private TextMeshProUGUI _buttonText;

    [Header("Resources (일반/재시작 배경)")]
    [SerializeField] private Sprite _backgroundNormal;
    [SerializeField] private Sprite _backgroundSamsara;

    private void Reset()
    {
        var allPanels = GetComponentsInChildren<CanvasGroup>(true);

        foreach (var panel in allPanels)
        {
            string objectName = panel.name.ToLower();

            if (_splashPanel == null && objectName.Contains("splash")) _splashPanel = panel;
            if (_titlePanel == null && objectName.Contains("title")) _titlePanel = panel;
        }

        if (_backgroundImage == null) _backgroundImage = GetComponentInChildren<Image>();
        if (_startButton == null) _startButton = GetComponentInChildren<Button>();
        if (_buttonText == null) _buttonText = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void OnDestroy()
    {
        _startButton.onClick.RemoveAllListeners();
    }

    // 초기 상태 설정 - 로고만 보이고 타이틀 숨김
    public void SetupInitialState()
    {
        _splashPanel.alpha = 1f;
        _titlePanel.alpha = 0f;
        _titlePanel.interactable = false; // 클릭 방지
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
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            _splashPanel.alpha = 1 - t;
            _titlePanel.alpha = t;

            // Yield에 토큰 전달 [핵심]
            // 만약 도중에 객체가 파괴되면 여기서 OperationCanceledException이 발생하며 
            // 아래 코드를 실행하지 않고 조용히 종료됩니다.
            await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken);
        }

        _splashPanel.alpha = 0f;
        _titlePanel.alpha = 1f;

        _titlePanel.interactable = true;
        _titlePanel.blocksRaycasts = true;

        _splashPanel.interactable = false;
        _splashPanel.blocksRaycasts = false;
    }

    // 게임시작 버튼 이벤트 연결
    public void SetOnClickStartButton(Action action)
    {
        _startButton.onClick.RemoveAllListeners();
        _startButton.onClick.AddListener(() => action.Invoke());
    }

    // 일반 시작화면 설정
    public void ShowNormalMode()
    {
        _backgroundImage.sprite = _backgroundNormal;
        string text = LocalizationUtils.GetString(key: "scene_intro_start_button_normal");
        _buttonText.text = text;
    }

    // 재시작 화면 설정
    public void ShowSamsaraMode()
    {
        _backgroundImage.sprite = _backgroundSamsara;
        string text = LocalizationUtils.GetString(key: "scene_intro_start_button_samsara");
        _buttonText.text = text;
    }
}