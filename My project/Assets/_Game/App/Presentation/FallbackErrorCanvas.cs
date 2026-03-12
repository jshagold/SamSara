using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 초기화 실패 시 표시되는 비상 에러 Canvas.
/// IPopupManager와 독립적으로 동작한다 (V-06).
/// 기본 상태: gameObject.SetActive(false).
/// </summary>
public class FallbackErrorCanvas : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _messageText;
    [SerializeField] private Button          _retryButton;
    [SerializeField] private Button          _quitButton;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    /// <summary>에러 Canvas를 표시하고 버튼 콜백을 등록한다.</summary>
    public void Show(Action onRetry, Action onQuit)
    {
        _retryButton.onClick.RemoveAllListeners();
        _quitButton.onClick.RemoveAllListeners();

        _retryButton.onClick.AddListener(() => onRetry?.Invoke());
        _quitButton.onClick.AddListener(() => onQuit?.Invoke());

        gameObject.SetActive(true);
    }

    /// <summary>에러 Canvas를 숨긴다.</summary>
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        _retryButton?.onClick.RemoveAllListeners();
        _quitButton?.onClick.RemoveAllListeners();
    }
}
