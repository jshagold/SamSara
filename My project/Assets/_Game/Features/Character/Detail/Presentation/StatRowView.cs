using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class StatRowView : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private TextMeshProUGUI _labelText; // "체력"
    [SerializeField] private TextMeshProUGUI _currentText;  // 현재 스탯수치
    [SerializeField] private TextMeshProUGUI _maxText;  // 최대 스탯수치
    [SerializeField] private Slider _statSlider;   // 스탯바 slider

    [Header("Settings")]
    [SerializeField] private string _valueFormat = "F0";

    private void Reset()
    {
        TextMeshProUGUI[] allTexts = GetComponentsInChildren<TextMeshProUGUI>(true);
        foreach(var text in allTexts)
        {
            var objName = text.gameObject.name.ToLower();
            if(_labelText == null && objName.Contains("label")) _labelText = text;
            if(_currentText == null && objName.Contains("current")) _currentText = text;
            if(_maxText == null && objName.Contains("max")) _maxText = text;
        }

        if(_statSlider == null) _statSlider = GetComponentInChildren<Slider>(true);
    }

    public void SetData(StatDisplayInfo statInfo)
    {
        _labelText.text = statInfo.Label;
        _currentText.text = statInfo.CurrentValue.ToString(_valueFormat);
        _maxText.text = statInfo.MaxValue.ToString(_valueFormat);
        _statSlider.value = statInfo.Ratio;
    }
}