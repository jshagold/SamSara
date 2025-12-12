using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class HUDView : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private OptionButtonView _optionButton; // 재사용 컴포넌트 연결
    [SerializeField] private DateDisplayView _dateDisplay;
    [SerializeField] private CurrencyView _currencyView;

    [Header("Animation Settings")]
    [SerializeField] private RectTransform _abovePanelRect; // 위로 움직일 묶음
    [SerializeField] private CanvasGroup _abovePanelCanvasGroup;
    [SerializeField] private RectTransform _leftPanelRect; // 왼쪽으로 움직일 묶음
    [SerializeField] private CanvasGroup _leftPanelCanvasGroup;

    [SerializeField] private float animDuration = 0.5f; // 속도

    [Tooltip("1.0 = 패널 크기(가로, 세로)만큼 이동, 1.1 = 10% 여유 버퍼")]
    [SerializeField][Range(1.0f, 1.5f)] private float hideOffsetRatio = 1.1f;

    private Vector2 _abovePanelVisiblePos;
    private Vector2 _abovePanelHiddenPos;
    private Vector2 _leftPanelVisiblePos;
    private Vector2 _leftPanelHiddenPos;

    private void Awake()
    {
        if(_abovePanelRect != null)
        {
            float abovePanelHeight = _abovePanelRect.rect.height;
            _abovePanelVisiblePos = _abovePanelRect.anchoredPosition;
            _abovePanelHiddenPos = new Vector2(
                _abovePanelVisiblePos.x, 
                _abovePanelVisiblePos.y + (abovePanelHeight * hideOffsetRatio)
            );
        }

        if(_leftPanelRect != null)
        {
            float leftPanelWidth = _leftPanelRect.rect.width;
            _leftPanelVisiblePos = _leftPanelRect.anchoredPosition;
            _leftPanelHiddenPos = new Vector2(
                _leftPanelVisiblePos.x - (leftPanelWidth * hideOffsetRatio),
                _leftPanelVisiblePos.y
            );
        }
    }

    private void Reset()
    {
        if (_optionButton == null) _optionButton = GetComponentInChildren<OptionButtonView>();
        if (_dateDisplay == null) _dateDisplay = GetComponentInChildren<DateDisplayView>();
        if (_currencyView == null) _currencyView = GetComponentInChildren<CurrencyView>();

        Debug.Log($"[TopHUDView] 에디터 자동 연결 완료 (Panel 연결 확인해야함): {name}");
    }

    // Presenter가 버튼 이벤트를 구독할 수 있게 연결 통로(Proxy)를 열어줍니다.
    public void SetOnClickOptionBtnAction(UnityAction action)
    {
        _optionButton.SetOnClickBtn(action);
    }

    public void UpdateCurreny(int amount)
    {
        _currencyView.SetMoneyText(amount);
    }

    public void UpdateDate(int date)
    {
        _dateDisplay.SetDateText(date);
    }

    // --- 애니메이션 코드 (HUD 효과) ---
    
    // 화면밖으로 나가기 (Hide)
    public async UniTask PlaySlideOut()
    {
        var token = this.GetCancellationTokenOnDestroy();

        var taskAbove = UniTask.CompletedTask;
        var taskLeft = UniTask.CompletedTask;

        if(_abovePanelRect != null)
        {
            if (_abovePanelCanvasGroup) _abovePanelCanvasGroup.interactable = false;

            taskAbove = _abovePanelRect.DOAnchorPos(_abovePanelHiddenPos, animDuration)
                .SetEase(Ease.InBack)
                .ToUniTask(cancellationToken: token);
        }

        if(_leftPanelRect != null)
        {
            if (_leftPanelCanvasGroup) _leftPanelCanvasGroup.interactable = false;

            taskLeft = _leftPanelRect.DOAnchorPos(_leftPanelHiddenPos, animDuration)
                .SetEase(Ease.InBack)
                .ToUniTask(cancellationToken: token);
        }

        await UniTask.WhenAll(taskAbove, taskLeft);

        if (_abovePanelRect != null) _abovePanelRect.gameObject.SetActive(false);
        if (_leftPanelRect != null) _leftPanelRect.gameObject.SetActive(false);
    }

    // 화면안으로 들어오기 (Show)
    public async UniTask PlaySlideIn()
    {
        var token = this.GetCancellationTokenOnDestroy();

        var taskAbove = UniTask.CompletedTask;
        var taskLeft = UniTask.CompletedTask;

        if (_abovePanelRect != null)
        {
            _abovePanelRect.gameObject.SetActive(true);
            taskAbove = _abovePanelRect.DOAnchorPos(_abovePanelVisiblePos, animDuration)
                .SetEase(Ease.OutBack)
                .ToUniTask(cancellationToken: token);
        }

        if (_leftPanelRect != null)
        {
            _leftPanelRect.gameObject.SetActive(true);
            taskLeft = _leftPanelRect.DOAnchorPos(_leftPanelVisiblePos, animDuration)
                .SetEase(Ease.OutBack)
                .ToUniTask(cancellationToken: token);
        }

        await UniTask.WhenAll(taskAbove, taskLeft);

       if(_abovePanelRect != null && _abovePanelCanvasGroup != null)
        {
            _abovePanelCanvasGroup.interactable = true;
        }
        if (_leftPanelRect != null && _leftPanelCanvasGroup != null)
        {
            _leftPanelCanvasGroup.interactable = true;
        }
    }
}
