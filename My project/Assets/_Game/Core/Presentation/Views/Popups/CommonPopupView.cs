using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CommonPopupView : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private GameObject _panelRoot;
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _descText;
    [SerializeField] private CanvasGroup _canvasGroup;  // 더블클릭 방지용


    [Header("Buttons")]
    [SerializeField] private Button _firstButton;
    [SerializeField] private TextMeshProUGUI _firstButtonText;
    [SerializeField] private Button _secondButton;
    [SerializeField] private TextMeshProUGUI _secondButtonText;

    private void Reset()
    {
        if (_panelRoot == null) _panelRoot = this.gameObject;
        if (_canvasGroup == null) _canvasGroup = GetComponent<CanvasGroup>();
        if (_canvasGroup == null && _panelRoot != null) _canvasGroup = _panelRoot.AddComponent<CanvasGroup>();

        // true 옵션을 넣으면 SetActive(false)된 객체도 전부 찾는다.
        var allTexts = GetComponentsInChildren<TextMeshProUGUI>(true);
        var allButtons = GetComponentsInChildren<Button>(true);

        foreach (var buttonComponent in allButtons)
        {
            string objectName = buttonComponent.name.ToLower();

            if (_firstButton == null && objectName.Contains("first")) _firstButton = buttonComponent;
            if (_secondButton == null && objectName.Contains("second")) _secondButton = buttonComponent;
        }

        foreach (var textComponent in allTexts)
        {
            // !!! hierarchy의 오브젝트 이름으로 구분 !!!
            string objectName = textComponent.name.ToLower();

            if (_titleText == null && objectName.Contains("title")) _titleText = textComponent;
            if (_descText == null && objectName.Contains("desc")) _descText = textComponent;
            if (_firstButtonText == null && objectName.Contains("first")) _firstButtonText = textComponent;
            if (_secondButtonText == null && objectName.Contains("second")) _secondButtonText = textComponent;
        }

        Debug.Log($"[{name}] component 자동 연결 완료");
    }

    // 첫번째 버튼 클릭 return true / 두번째 버튼 클릭 return false
    public async UniTask<bool> ShowPopupAsync(string title, string message, string firstBtnText, string secondBtnText)
    {
        var token = this.GetCancellationTokenOnDestroy();

        _titleText.text = title;
        _descText.text = message;
        _firstButtonText.text = firstBtnText;
        _secondButtonText.text = secondBtnText;

        bool isOneButton = string.IsNullOrEmpty(secondBtnText);
        _secondButton.gameObject.SetActive(!isOneButton);
        if (!isOneButton) _secondButtonText.text = secondBtnText;

        // 화면 켜기 && 터치 활성화
        _panelRoot.SetActive(true);
        if(_canvasGroup != null) _canvasGroup.interactable = true;

        UniTask firstBtnTask = _firstButton.OnClickAsync(token);

        // 취소 버튼이 없으면 영원히 안 눌릴 테니 그냥 멈춰있는 Task를 줍니다.
        UniTask secondBtnTask;
        if (isOneButton || _secondButton)
        {
            secondBtnTask = UniTask.Never(token);
        } 
        else
        {
            secondBtnTask = _secondButton.OnClickAsync(token);
        }

        // WhenAny: 둘 중 하나라도 끝나면 그 인덱스를 반환 (0: confirm, 1: cancel)
        int winIndex = await UniTask.WhenAny(firstBtnTask, secondBtnTask);

        if(_canvasGroup != null) _canvasGroup.interactable = false; // 더블클릭 방지
        _panelRoot.SetActive(false);

        // 첫번째 버튼 클릭 return true / 두번째 버튼 클릭 return false
        return winIndex == 0;
    }

}