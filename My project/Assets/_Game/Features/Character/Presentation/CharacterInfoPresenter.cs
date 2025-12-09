using UnityEngine;

// View <-> UseCase 사이를 연결하는 Presenter
public class CharacterInfoPresenter
{
	private readonly CharacterInfoView _view;
	private readonly GetMainCharacterInfoUseCase _getMainCharacterInfoUseCase;

	public CharacterInfoPresenter(
        CharacterInfoView view,
        GetMainCharacterInfoUseCase getMainCharacterInfoUseCase)

    {
        _view = view;
        _getMainCharacterInfoUseCase = getMainCharacterInfoUseCase;
    }

	// 씬 시작 시 한 번 호출 (초기 상태 표시)
	public void Initialize()

    {
        LoadAndShowCharacter();
    }

	// 버튼 눌렀을 때 호출
	public void OnClickRefresh()

    {
        Debug.Log("캐릭터 정보 새로고침");
        LoadAndShowCharacter();
    }

	private void LoadAndShowCharacter()

    {
        // Domain UseCase에 요청해서 데이터 가져오기
        CharacterDef character = _getMainCharacterInfoUseCase.Execute();

        _view.ShowCharacter(character);
    }
}