using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatRowView : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private TextMeshProUGUI _nameText; // "체력"
    [SerializeField] private TextMeshProUGUI _currentText;  // 현재 스탯수치
    [SerializeField] private TextMeshProUGUI _maxText;  // 최대 스탯수치
    [SerializeField] private Slider _gaugeSlider;   // 게이지 slider

    private void Reset()
    {
        
    }

    public void SetData()
    {

    }
}