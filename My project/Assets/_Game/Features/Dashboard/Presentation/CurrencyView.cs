using UnityEngine.Events;
using UnityEngine;
using TMPro;

public class CurrencyView : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI _moneyText;

    [Header("Settings")]
    [SerializeField] private string currencySuffix = "G";

    public void SetMoneyText(int amount)
    {
        // N0 : 1000 단위 콤마 포맷
        // 1000 -> "1,000 G"로 변환
        _moneyText.text = $"{amount:N0}{currencySuffix}";
    }

}