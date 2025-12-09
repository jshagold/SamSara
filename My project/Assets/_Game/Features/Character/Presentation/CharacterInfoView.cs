using TMPro;
using UnityEngine;

// 화면에 캐릭터 기본 정보를 보여주는 View
public class CharacterInfoView : MonoBehaviour
{
	[Header("Texts")]
    [SerializeField] private TextMeshProUGUI _nameText;
	[SerializeField] private TextMeshProUGUI _hpText;
	[SerializeField] private TextMeshProUGUI _strengthText;
	[SerializeField] private TextMeshProUGUI _toughnessText;
	[SerializeField] private TextMeshProUGUI _agilityText;

	[Header("Button")]
    [SerializeField] private OptionButtonView _refreshButton;

	private CharacterInfoPresenter _presenter;

	// Bootstrapper에서 Presenter를 넘겨줄 때 호출
	public void SetPresenter(CharacterInfoPresenter presenter)

    {
        _presenter = presenter;

        if (_refreshButton != null)
        {
            // 공용 OptionButtonView의 OnClicked 이벤트를 Presenter로 전달
            _refreshButton.OnClicked += OnRefreshClicked;
        }
    }

	private void OnDestroy()

    {
        if (_refreshButton != null)
        {
            _refreshButton.OnClicked -= OnRefreshClicked;
        }
    }

	private void OnRefreshClicked()

    {
        _presenter?.OnClickRefresh();
    }

	// Presenter가 받은 CharacterDef를 실제 UI에 반영
	public void ShowCharacter(CharacterDef character)

    {
        if (character == null || character.baseStats == null)
        {
            _nameText.text = "-";
            _hpText.text = "-";
            _strengthText.text = "-";
            _toughnessText.text = "-";
            _agilityText.text = "-";
            return;
        }

        _nameText.text = character.displayName;
        _hpText.text = $"{character.baseStats.healthCurrent}/{character.baseStats.healthMax}";
        _strengthText.text = character.baseStats.strength.ToString();
        _toughnessText.text = character.baseStats.toughness.ToString();
        _agilityText.text = character.baseStats.agility.ToString();
    }
}