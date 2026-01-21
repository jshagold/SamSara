using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MainSceneCharacterSummaryView : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private CharacterAssetConfig _characterAssetConfig;

    [Header("Character Info")]
    [SerializeField] private Image portraitImage;
    [SerializeField] private Slider hpSlider;

    [Header("Daily Actions (Coin UI)")]
    [SerializeField] private Transform iconContainer; // Grid나 Horizontal Layout이 달린 부모 객체
    [SerializeField] private GameObject actionIconPrefab; // 동전 프리팹 (ON/OFF 기능 포함)

    // 이미 생성된 아이콘들 재사용을 위한 리스트
    private List<ActionIconView> _spawnedIcons = new List<ActionIconView>();

    public void Render(MainSceneCharacterSummaryInfo data)
    {
        // 캐릭터 정보 갱신
        Sprite foundSprite = _characterAssetConfig.GetPortrait(data.CharacterId);
        portraitImage.sprite = foundSprite;
        hpSlider.maxValue = data.MaxHp.Value;
        hpSlider.value = data.CurrentHp.Value;

        // 행동 횟수 UI 갱신
        UpdateActionIcons(data.ActionFlags);
    }

    private void UpdateActionIcons(bool[] flags)
    {
        // A. 개수 맞추기 (모자르면 더 만들고, 남으면 끄기)
        // (간단하게 구현하기 위해 매번 싹 지우고 다시 만드는 방식보다는, 풀링 방식 추천)

        // 필요한 개수만큼 프리팹 확보
        while (_spawnedIcons.Count < flags.Length)
        {
            GameObject obj = Instantiate(actionIconPrefab, iconContainer);
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