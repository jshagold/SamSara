using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EvolutionNodeSlotView : MonoBehaviour, IPointerClickHandler
{
    [Header("Components")]
    [SerializeField] private RectTransform _rectTransform;

    [Header("Base Visuals")]
    [SerializeField] private Image _portraitImage;
    [SerializeField] private Image _frameImage;

    [Header("Overlays")]
    [SerializeField] private GameObject _lockOverlayObj;    // Locked
    [SerializeField] private GameObject _completedOverlayObj;   // Completed

    [Header("Color Settings")]
    [SerializeField] private Color _normalPortraitColor = Color.white;
    [SerializeField] private Color _dimmedPortraitColor = new Color(0.4f, 0.4f, 0.4f, 1f);  // 어두운 처리

    [Header("Frame Colors")]
    [SerializeField] private Color _currentFrameColor = new Color(0f, 1f, 0f, 1f);  // 밝은 녹색 등 (가장 눈에 띄게)
    [SerializeField] private Color _possibleFrameColor = new Color(1f, 0.9f, 0.5f, 1f); // 연한 노랑 등 (Current보다 덜 눈에 띄게)
    [SerializeField] private Color _impossibleFrameColor = new Color(0.3f, 0.3f, 0.3f, 1f); // 어두운 색
    [SerializeField] private Color _defaultFrameColor = Color.white;    // 기본

    // Logic
    private Action _onClickAction;     // 클릭 action
    private bool _isInteractable = true;    // 클릭터치가 되는지

    private void Reset()
    {
        Image[] allImages = GetComponentsInChildren<Image>(true);
        foreach(Image image in allImages)
        {
            string objName = image.gameObject.name.ToLower();
            if (_portraitImage == null && objName.Contains("portrait")) _portraitImage = image;
            if (_frameImage == null && objName.Contains("frame")) _frameImage = image;
        }

        Transform[] allTransforms = GetComponentsInChildren<Transform>(true);
        foreach(Transform tr in allTransforms)
        {
            string objName = tr.gameObject.name.ToLower();
            if (_lockOverlayObj == null && objName.Contains("lock")) _lockOverlayObj = tr.gameObject;
            if (_completedOverlayObj == null && objName.Contains("complete")) _completedOverlayObj = tr.gameObject;
        }
    }
    
    public void SetData(EvolutionStateType stateType, Sprite icon)
    {
        SetIcon(icon: icon);
        UpdateVisualState(state: stateType);
    }


    public void SetOnClick(Action onclick)
    {
        _onClickAction = onclick;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if(!_isInteractable) return;
        _onClickAction?.Invoke();
    }

    public void SetPosition(Vector2 position)
    {
        _rectTransform.anchoredPosition = position;
    }

    // --- Internal Login ---
    private void SetIcon(Sprite icon)
    {
        _portraitImage.sprite = icon;
    }

    private void UpdateVisualState(EvolutionStateType state)
    {
        SetOverlayActive(_lockOverlayObj, false);
        SetOverlayActive(_completedOverlayObj, false);
        SetPortraitColor(_normalPortraitColor);
        SetFrameColor(_defaultFrameColor);

        _isInteractable = true;

        switch (state)
        {
            case EvolutionStateType.Current:
                SetPortraitColor(_normalPortraitColor);
                SetFrameColor(_currentFrameColor);
                break;

            case EvolutionStateType.Impossible:
                SetPortraitColor(_normalPortraitColor);
                SetFrameColor(_impossibleFrameColor);
                break;

            case EvolutionStateType.Possible:
                SetPortraitColor(_normalPortraitColor);
                SetFrameColor(_possibleFrameColor);
                break;

            case EvolutionStateType.Completed:
                SetPortraitColor(_dimmedPortraitColor);
                SetOverlayActive(_completedOverlayObj, true);
                break;

            case EvolutionStateType.Locked:
                SetPortraitColor(_dimmedPortraitColor);
                SetOverlayActive(_lockOverlayObj, true);
                break;
        }
    }

    private void SetPortraitColor(Color color)
    {
        _portraitImage.color = color;
    }

    private void SetFrameColor(Color color)
    {
        _frameImage.color = color;
    }

    private void SetOverlayActive(GameObject overlay, bool isActive)
    {
        overlay.SetActive(isActive);
    }
}