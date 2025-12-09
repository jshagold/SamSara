using TMPro;
using UnityEngine;

public class DateDisplayView : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI _dateText;

    [Header("Settings")]
    [SerializeField] private string currencySuffix = "Day ";

    public void SetDateText(int date)
    {
        // 100 -> "Day 100"으로 변환
        _dateText.text = $"{currencySuffix}{date}";
    }

}