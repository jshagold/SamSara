using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainBackgroundView : MonoBehaviour
{
    [Header("Target Image")]
    [SerializeField] private Image _backgroundImage;

    [Header("Resources")]
    [SerializeField] private List<BackgroundData> _backgrounds;

    // 외부(Presenter)에서 "야, 밤 배경으로 바꿔" 라고 호출하는 메서드
    public void SetBackground(BackgroundType type)
    {
        // 리스트에서 해당 타입에 맞는 Sprite를 찾음
        var data = _backgrounds.Find(x => x.Type == type);

        if (data != null && data.Sprite != null)
        {
            _backgroundImage.sprite = data.Sprite;
        }
        else
        {
            Debug.LogWarning($"[BackgroundView] 해당 타입의 이미지가 없습니다: {type}");
        }
    }
}
