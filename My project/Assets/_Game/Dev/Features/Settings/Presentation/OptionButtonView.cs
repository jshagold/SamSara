using System;
using UnityEngine;
using UnityEngine.UI;

public class OptionButtonView : MonoBehaviour
{
    [SerializeField] private Button _button;

    private void Reset()
    {
        if ( _button == null ) _button = GetComponent<Button>();
    }

    private void OnDestroy()
    {
        _button.onClick.RemoveAllListeners();
    }

    public void SetOnClickAction(Action action)
    {
        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(() => action.Invoke());    
    }
}