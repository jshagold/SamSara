using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class HUDView : MonoBehaviour
{
    private readonly string _logClass = $"[{nameof(HUDView)}]";
    
    [Header("Move Panel Components (움직이는 패널)")]
    [SerializeField] private RectTransform _abovePanelRect; // 위로 움직일 묶음
    [SerializeField] private CanvasGroup _abovePanelCanvasGroup;
    [SerializeField] private RectTransform _leftPanelRect; // 왼쪽으로 움직일 묶음
    [SerializeField] private CanvasGroup _leftPanelCanvasGroup;

    [Header("Static Components (고정된 View)")]
    [SerializeField] private HUDOnOffButtonView _hudOnOffButton;

    [Header("Panel Content Components (패널 내부 View 들)")]
    [SerializeField] private DateDisplayView _dateDisplay;
    [SerializeField] private CurrencyView _currencyView;
    [SerializeField] private OptionButtonView _optionButton;

    [Header("Animation Settings")]
    [SerializeField] private float _animDuration = 0.5f;
    [Tooltip("1.0 = 패널 크기(가로, 세로)만큼 이동, 1.1 = 10% 여유 버퍼")]
    [SerializeField][Range(1.0f, 1.5f)] private float _hideOffsetRatio = 1.1f;

    [Header("Character List View")]
    [SerializeField] private MainSceneCharacterListView _characterListView;

    // Presenter가 사용할 수 있게 프로퍼티로 노출
    public OptionButtonView OptionButton => _optionButton;
    public HUDOnOffButtonView OnOffButton => _hudOnOffButton;
    public MainSceneCharacterListView CharacterListView => _characterListView;

    // 캐릭터 뷰 리스트 (오브젝트 풀링, 재사용 목적)
    private List<MainSceneCharacterSummaryView> _spawnedSummaryViews = new List<MainSceneCharacterSummaryView>();

    private Vector2 _abovePanelVisiblePos;
    private Vector2 _abovePanelHiddenPos;
    private Vector2 _leftPanelVisiblePos;
    private Vector2 _leftPanelHiddenPos;

    private void Awake()
    {
        if (_abovePanelRect == null)
            throw new InvalidOperationException($"{_logClass} _abovePanelRect must be assigned in Inspector.");
        if (_leftPanelRect == null)
            throw new InvalidOperationException($"{_logClass} _leftPanelRect must be assigned in Inspector.");

        float abovePanelHeight = _abovePanelRect.rect.height;
        _abovePanelVisiblePos = _abovePanelRect.anchoredPosition;
        _abovePanelHiddenPos = new Vector2(
            _abovePanelVisiblePos.x,
            _abovePanelVisiblePos.y + (abovePanelHeight * _hideOffsetRatio)
        );

        float leftPanelWidth = _leftPanelRect.rect.width;
        _leftPanelVisiblePos = _leftPanelRect.anchoredPosition;
        _leftPanelHiddenPos = new Vector2(
            _leftPanelVisiblePos.x - (leftPanelWidth * _hideOffsetRatio),
            _leftPanelVisiblePos.y
        );
    }

    private void Reset()
    {
        if (_hudOnOffButton == null) _hudOnOffButton = GetComponentInChildren<HUDOnOffButtonView>();
        if (_dateDisplay == null) _dateDisplay = GetComponentInChildren<DateDisplayView>();
        if (_currencyView == null) _currencyView = GetComponentInChildren<CurrencyView>();
        if (_optionButton == null) _optionButton = GetComponentInChildren<OptionButtonView>();
        if (_characterListView == null) _characterListView = GetComponentInChildren<MainSceneCharacterListView>();

        Debug.Log($"{_logClass} Reset 완료 (Panel 연결 확인): {name}");
    }

    public void SetOnClickHUDOnOffBtnAction(Action action)
    {
        _hudOnOffButton.SetOnClicked(action);
    }

    public void SetOnClickOptionBtnAction(Action action)
    {
        _optionButton.SetOnClickAction(action);
    }

    // --- 데이터 갱신 ---
    public void UpdateCurrency(int amount) => _currencyView.SetMoneyText(amount);
    public void UpdateDate(int date) => _dateDisplay.SetDateText(date);

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

            taskAbove = _abovePanelRect.DOAnchorPos(_abovePanelHiddenPos, _animDuration)
                .SetEase(Ease.InBack)
                .ToUniTask(cancellationToken: token);
        }

        if(_leftPanelRect != null)
        {
            if (_leftPanelCanvasGroup) _leftPanelCanvasGroup.interactable = false;

            taskLeft = _leftPanelRect.DOAnchorPos(_leftPanelHiddenPos, _animDuration)
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
            taskAbove = _abovePanelRect.DOAnchorPos(_abovePanelVisiblePos, _animDuration)
                .SetEase(Ease.OutBack)
                .ToUniTask(cancellationToken: token);
        }

        if (_leftPanelRect != null)
        {
            _leftPanelRect.gameObject.SetActive(true);
            taskLeft = _leftPanelRect.DOAnchorPos(_leftPanelVisiblePos, _animDuration)
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
