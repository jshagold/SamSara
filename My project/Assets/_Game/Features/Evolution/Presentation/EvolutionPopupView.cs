using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EvolutionPopupView : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private GameObject _panelRoot;
    [SerializeField] private Image _portraitImg;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _descText;
    [SerializeField] private TextMeshProUGUI _requireLabel;
    [SerializeField] private TextMeshProUGUI _requireText;
    [SerializeField] private TextMeshProUGUI _evolutionText;
    [SerializeField] private Button _btnEvolution;
    [SerializeField] private Button _btnClose;
    [SerializeField] private Button _btnBackgroundDim;

    private void Reset()
    {
        Image[] images = GetComponentsInChildren<Image>(true);
        foreach (Image image in images)
        {
            string objName = image.gameObject.name.ToLower();
            if (_portraitImg == null && objName.Contains("portrait")) _portraitImg = image;
        }

        TextMeshProUGUI[] allTexts = GetComponentsInChildren<TextMeshProUGUI>(true);
        foreach (TextMeshProUGUI text in allTexts)
        {
            string objName = text.gameObject.name.ToLower();
            if (_nameText == null && objName.Contains("name")) _nameText = text;
            if (_descText == null && objName.Contains("desc")) _descText = text;
            if(_requireLabel == null && objName.Contains("req") && objName.Contains("label")) _requireLabel = text;
            if (_requireText == null && objName.Contains("req") && objName.Contains("text")) _requireText = text;
        }

        Button[] buttons = GetComponentsInChildren<Button>(true);
        foreach (Button btn in buttons)
        {
            string objName = btn.gameObject.name.ToLower();
            if (_btnClose == null && objName.Contains("close")) _btnClose = btn;
            if (_btnBackgroundDim == null && objName.Contains("dimmed")) _btnBackgroundDim = btn;
            if (_btnEvolution == null && objName.Contains("evolve")) _btnEvolution = btn;
        }
    }

    private void Awake()
    {
        ClosePopup();
    }

    /// <summary>
    /// 닫기 버튼 등의 고정적인 이벤트를 설정합니다. (Presenter 초기화 시 1회 호출 추천)
    /// </summary>
    public void SetCloseActions(Action onClickClose)
    {
        // 닫기 버튼
        _btnClose.onClick.RemoveAllListeners();
        _btnClose.onClick.AddListener(() =>
        {
            ClosePopup();
            onClickClose?.Invoke();
        });

        // 배경 딤 처리 버튼
        _btnBackgroundDim.onClick.RemoveAllListeners();
        _btnBackgroundDim.onClick.AddListener(() =>
        {
            ClosePopup();
            onClickClose?.Invoke();
        });
    }

    /// <summary>
    /// 팝업을 열고 데이터를 갱신합니다.
    /// </summary>
    /// <param name="name">노드 이름</param>
    /// <param name="desc">설명</param>
    /// <param name="requirements">요구 조건 텍스트</param>
    /// <param name="iconSprite">아이콘 이미지</param>
    /// <param name="canEvolve">진화 가능 여부 (버튼 활성화용)</param>
    /// <param name="onEvolveClick">진화 버튼 클릭 시 실행할 액션</param>
    public void OpenPopup(
        string requireLabel, 
        string evolveLabel, 
        string name, 
        string desc,  
        string requirements, 
        Sprite iconSprite, 
        bool canEvolve, 
        Action onEvolveClick)
    {
        _nameText.text = name;
        _descText.text = desc;
        _requireLabel.text = requireLabel;
        _requireText.text = requirements;
        _portraitImg.sprite = iconSprite;
        _evolutionText.text = evolveLabel;

        _btnEvolution.interactable = canEvolve;
        _btnEvolution.onClick.RemoveAllListeners(); // 재사용 시 이전 이벤트 제거 필수
        _btnEvolution.onClick.AddListener(() =>
        {
            // 버튼을 누르면 팝업을 닫을지 말지는 기획에 따라 결정 (여기서는 유지)
            onEvolveClick?.Invoke();
        });

        // 패널 활성화
        _panelRoot.SetActive(true);
    }


    public void ClosePopup()
    {
        if (_panelRoot != null) _panelRoot.SetActive(false);
        else gameObject.SetActive(false);
    }
}