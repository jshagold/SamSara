using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainBackgroundView : MonoBehaviour
{
    private readonly string _logClass = $"[{nameof(MainBackgroundView)}]";

    [Header("Target Image")]
    [SerializeField] private Image _backgroundImage;

    [Header("Resources")]
    [SerializeField] private List<BackgroundData> _backgrounds;

    public void SetBackground(BackgroundType type)
    {
        var data = _backgrounds?.Find(x => x.Type == type);

        if (data != null && data.Sprite != null)
        {
            _backgroundImage.sprite = data.Sprite;
        }
        else
        {
            Debug.LogWarning($"{_logClass} Background not found: {type}");
        }
    }
}
