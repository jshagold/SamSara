using System;
using UnityEngine;
using UnityEngine.UI;

public class OptionButtonView : MonoBehaviour
{
    [SerializeField] private Button _button;

    // 외부(Presenter)에서 구독할 이벤트
    public event Action OnClicked;

    private void Awake()
    {
        // 안전장치: 실수로 인스펙터 연결 안 했을 경우 자동 찾기
        if (_button == null)
            _button = GetComponent<Button>();

        // 람다 대신 명명된 메서드 등록 (관리가 더 깔끔함)
        _button.onClick.AddListener(() => OnClicked?.Invoke());
    }

    private void OnDestroy()
    {
        // 1. 유니티 버튼 리스너 정리
        if (_button != null)
            _button.onClick.RemoveAllListeners();

        // 2. [중요] 나를 구독하고 있던 Presenter들과의 연결을 끊음
        // 이걸 안 하면 Presenter가 죽은 View를 계속 붙들고 있을 수 있음
        OnClicked = null;
    }
}
