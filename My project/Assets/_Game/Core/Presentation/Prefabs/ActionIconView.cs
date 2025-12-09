using UnityEngine;
using UnityEngine.UI;

public class ActionIconView : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Sprite onSprite;
    [SerializeField] private Sprite offSprite;

    public void SetActiveState(bool isOn)
    {
        iconImage.sprite = isOn ? onSprite : offSprite;

        // 색으로 조정하겠다면
        // iconImage.color = isOn ? Color.white : Color.gray;
    }
}