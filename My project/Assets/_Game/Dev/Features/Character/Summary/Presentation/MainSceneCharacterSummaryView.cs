using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MainSceneCharacterSummaryView : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private CharacterAssetConfig _characterAssetConfig;

    [Header("Character Info")]
    [SerializeField] private Image _portraitImage;
    [SerializeField] private Slider _hpSlider;

    [Header("Daily Actions (Coin UI)")]
    [SerializeField] private Transform _iconContainer;
    [SerializeField] private GameObject _actionIconPrefab;

    // 이미 생성된 아이콘들 재사용을 위한 리스트
    private List<ActionIconView> _spawnedIcons = new List<ActionIconView>();

    private void Reset()
    {
        Image[] images = GetComponentsInChildren<Image>(true);
        foreach (Image img in images)
        {
            if (_portraitImage == null && img.gameObject.name.ToLower().Contains("portrait")) _portraitImage = img;
        }
        _hpSlider = GetComponentInChildren<Slider>(true);
    }

    public void Render(MainSceneCharacterSummaryInfo data)
    {
        if (data == null)
            throw new System.ArgumentNullException(nameof(data));
        Sprite foundSprite = _characterAssetConfig.GetPortrait(data.CharacterId);
        _portraitImage.sprite = foundSprite;
        _hpSlider.maxValue = data.MaxHp.Value;
        _hpSlider.value = data.CurrentHp.Value;

        UpdateActionIcons(data.ActionFlags);
    }

    private void UpdateActionIcons(bool[] flags)
    {
        while (_spawnedIcons.Count < flags.Length)
        {
            GameObject obj = Instantiate(_actionIconPrefab, _iconContainer);
            _spawnedIcons.Add(obj.GetComponent<ActionIconView>());
        }

        // B. 상태 설정 (ON/OFF)
        for (int i = 0; i < _spawnedIcons.Count; i++)
        {
            if (i < flags.Length)
            {
                _spawnedIcons[i].gameObject.SetActive(true);
                // true면 켜진 동전, false면 꺼진 동전
                _spawnedIcons[i].SetActiveState(flags[i]);
            }
            else
            {
                // 데이터보다 아이콘이 많으면 숨김
                _spawnedIcons[i].gameObject.SetActive(false);
            }
        }
    }
}