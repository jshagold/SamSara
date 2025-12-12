using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class HUDOnOffButtonView : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Button _button;
    [SerializeField] private Image _iconImage;

    [Header("Resources")]
    [SerializeField] private Sprite _openSprite;
    [SerializeField] private Sprite _closeSprite;

    private void Reset()
    {
        if( _button == null ) _button = GetComponent<Button>();
        // 이미지 컴포넌트가 버튼의 자식에 있는지 본인에게 있는지 확인해서 할당
        if(_iconImage == null ) _iconImage = transform.GetChild(0).GetComponent<Image>();
    }

    private void OnDestroy()
    {
        if(_button != null) _button.onClick.RemoveAllListeners();
    }

    public void SetState(bool isHudOpen)
    {
        if (isHudOpen)
        {
            _iconImage.sprite = _closeSprite;
        }
        else
        {
            _iconImage.sprite= _openSprite;
        }
    }

    public void SetOnClicked(UnityAction action)
    {
        _button.onClick.RemoveAllListeners();
        _button.onClick.AddListener(action);
    }
}