using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterDetailView : MonoBehaviour
{
    [Header("Top Area")]
    [SerializeField] private Button _backButton;
    [SerializeField] private Image _portraitImage;
    [SerializeField] private Button _evolutionSceneButton;

    [Header("Contents Area")]
    [SerializeField] private Image _titleBg;
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private Image _characterNameBg;
    [SerializeField] private TextMeshProUGUI _characterNameText;
    [SerializeField] private ScrollRect _scrollRect;

    [Header("Sub Views")]
    [SerializeField] private StatListView _statListView;
    [SerializeField] private SkillListView _skillListView;
    [SerializeField] private InventoryView _inventoryView;

    // Presenter가 사용할 수 있게 프로퍼티로 노출
    public StatListView StatListView => _statListView;
    public SkillListView SkillListView => _skillListView;
    public InventoryView InventoryView => _inventoryView;

    private void Reset()
    {
        Button[] buttons = GetComponentsInChildren<Button>(true);
        foreach(Button button in buttons)
        {
            string objName = button.gameObject.name.ToLower();
            if(_backButton == null && objName.Contains("back")) _backButton = button;
            if(_evolutionSceneButton == null && objName.Contains("evolution")) _evolutionSceneButton = button;
        }

        Image[] images = GetComponentsInChildren<Image>(true);
        foreach(Image image in images)
        {
            string objName = image.gameObject.name.ToLower();
            if (_portraitImage == null && objName.Contains("portrait")) _portraitImage = image;
            if (_titleBg == null && objName.Contains("title")) _titleBg = image;
            if (_characterNameBg == null && objName.Contains("character")) _characterNameBg = image;
        }

        TextMeshProUGUI[] texts = GetComponentsInChildren<TextMeshProUGUI>(true);
        foreach(TextMeshProUGUI text in texts)
        {
            string objName = text.gameObject.name.ToLower();
            if(_titleText == null && objName.Contains("title")) _titleText = text;
            if(_characterNameText == null && objName.Contains("character")) _characterNameText = text;
        }

        if(_scrollRect == null) _scrollRect = GetComponentInChildren<ScrollRect>(true);

        if (_statListView == null) _statListView = GetComponentInChildren<StatListView>(true);
        if (_skillListView == null) _skillListView = GetComponentInChildren<SkillListView>(true);
        if (_inventoryView == null) _inventoryView = GetComponentInChildren<InventoryView>(true);
    }

    public void OnClickBackButton(Action action)
    {
        _backButton.onClick.RemoveAllListeners();
        _backButton.onClick.AddListener(() => action.Invoke());
    }

    public void OnClickEvoSceneButton(Action action)
    {
        _evolutionSceneButton.onClick.RemoveAllListeners();
        _evolutionSceneButton.onClick.AddListener(() => action.Invoke());
    }

    public void SetTitleText(string text)
    {
        _titleText.text = text;
    }

    public void SetCharacterName(string name)
    {
        _characterNameText.text = name;
    }

    public void SetCharacterImage(Sprite sprite)
    {
        _portraitImage.sprite = sprite;
    }

    public void ResetScroll()
    {
        if(_scrollRect != null)
        {
            _scrollRect.verticalNormalizedPosition = 1f;
        }
    }
}