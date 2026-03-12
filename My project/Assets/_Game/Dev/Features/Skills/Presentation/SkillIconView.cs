using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillIconView : MonoBehaviour
{
    [Header("기본")]
    [SerializeField] private GameObject _contentsRoot;  // 빈 스킬슬롯 처리를 위한 Root
    [SerializeField] private Image _iconFrame;
    [SerializeField] private Image _iconImage;
    [SerializeField] private Button _iconButton;

    [Header("전투")]
    [SerializeField] private Image _cooldownDim;    // 쿨타임 배경
    [SerializeField] private TextMeshProUGUI _cooldownText; // 쿨타임 텍스트
    [SerializeField] private GameObject _selectedEffect;    // 스킬 선택시 하이라이트 이펙트

    private void Reset()
    {
        Image[] allImages = GetComponentsInChildren<Image>(true);
        foreach(Image image in allImages)
        {
            string objName = image.gameObject.name.ToLower();

            if(_iconFrame == null && objName.Contains("frame")) _iconFrame = image;
            if(_iconImage == null && objName.Contains("image")) _iconImage = image;
            
            // [주의] 쿨타임 이미지는 반드시 Image Type이 Filled여야 함
            if(_cooldownDim == null && objName.Contains("cooldown")) _cooldownDim = image;
        }

        if(_cooldownText == null) _cooldownText = GetComponentInChildren<TextMeshProUGUI>(true);
        if(_iconButton == null) _iconButton = GetComponent<Button>();

        // TODO SelectedEffect 설정해야함
    }

    public void SetData(SkillInfo info, Sprite iconImage)
    {
        if(info == null || iconImage == null)
        {
            if(_contentsRoot != null) _contentsRoot.SetActive(false);
            if(_iconFrame != null) _iconFrame.enabled = true;
            return;
        }

        if(_contentsRoot != null) _contentsRoot.SetActive(true);
        if(_iconFrame != null) _iconFrame.enabled = true;

        _iconImage.sprite = iconImage;
        _iconImage.enabled = true;

        ResetVisualState();
    }

    public void SetOnClickIcon(Action action)
    {
        _iconButton.onClick.RemoveAllListeners();

        if(action != null)
        {
            _iconButton.onClick.AddListener(() => action.Invoke());
            _iconButton.interactable = true;
        }
        else
        {
            _iconButton.interactable = false;
        }
    }

    // 전투 UI 초기화
    private void ResetVisualState()
    {
        if(_cooldownDim != null) _cooldownDim.fillAmount = 0f;
        if(_cooldownText != null) _cooldownText.gameObject.SetActive(false);
        if(_selectedEffect != null) _selectedEffect.SetActive(false);
    }

    // TODO === 전투 로직 ===
  
    // 쿨타임설정
    public void SetCooldown(float fillAmount, string cooldownText)
    {
        bool isCooldown = fillAmount > 0f;

        if(_cooldownDim != null) _cooldownDim.fillAmount = fillAmount;
        if(_cooldownText != null)
        {
            _cooldownText.gameObject.SetActive(isCooldown);
            if (isCooldown) _cooldownText.text = cooldownText;
        }
    }

    // 스킬선택시 UI 수정
    public void SetSelected(bool isSelected)
    {
        if(_selectedEffect != null) _selectedEffect.SetActive(isSelected);
    }
}