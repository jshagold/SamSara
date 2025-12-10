using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;

public class MaintenanceButtonView : MonoBehaviour
{
    [SerializeField] private Button btn;
    [SerializeField] private Image iconImg;
    [SerializeField] private TextMeshProUGUI textMesh;

    [Header("Resources")]
    [SerializeField] private Sprite normalSprite;


    public void SetOnClickAction(UnityAction action)
    {
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(action);
    }

    // --- 화면 갱신용 함수들 ---
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void ShowNormalMode()
    {
        gameObject.SetActive(true);
        iconImg.sprite = normalSprite;
        string text = LocalizationUtils.GetString(key: "scene_main_maintenance_button");
        textMesh.text = text;
    }
 
}