using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class OptionButtonView : MonoBehaviour
{
    [SerializeField] private Button _button;

    private void Reset()
    {
        // 안전장치: 실수로 인스펙터 연결 안 했을 경우 자동 찾기
        if (_button == null) _button = GetComponent<Button>();
    }

    private void OnDestroy()
    {
        // 1. 유니티 버튼 리스너 정리
        if (_button != null) _button.onClick.RemoveAllListeners();
    }

    public void SetOnClickBtn(UnityAction action)
    {
        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(action);
    }
}
