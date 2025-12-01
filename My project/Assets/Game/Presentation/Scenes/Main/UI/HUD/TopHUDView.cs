using System;
using TMPro;
using UnityEngine;

public class TopHUDView : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private OptionButtonView _optionButton; // 재사용 컴포넌트 연결
    [SerializeField] private TextMeshProUGUI _statusText;

    // Presenter가 버튼 이벤트를 구독할 수 있게 연결 통로(Proxy)를 열어줍니다.
    public event Action OnOptionClicked
    {
        add => _optionButton.OnClicked += value;
        remove => _optionButton.OnClicked -= value;
    }

    public void UpdateStatus(string text)
    {
        _statusText.text = text;
    }
}
