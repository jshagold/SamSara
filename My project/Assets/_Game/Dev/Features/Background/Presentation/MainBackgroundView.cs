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

        if (data == null)
            throw new System.InvalidOperationException($"{_logClass} Background not found: {type}");

        _backgroundImage.sprite = data.Sprite;
    }
}
