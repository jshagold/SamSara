using UnityEngine;

// CharacterInfo 시스템을 초기화하는 Bootstrapper
public class CharacterInfoBootstrapper : MonoBehaviour
{
	[SerializeField] private CharacterInfoView _characterInfoView;

	public void Initialize()

    {
        // Data 레이어 객체 생성
        ICharacterDefRepository characterDefRepository = new CharacterDefRepository();

        // Domain UseCase 생성
        var getMainCharacterInfoUseCase =
                new GetMainCharacterInfoUseCase(characterDefRepository);

        // Presenter 생성
        var presenter = new CharacterInfoPresenter(_characterInfoView, getMainCharacterInfoUseCase);

        // View에 Presenter 연결
        _characterInfoView.SetPresenter(presenter);

        // 초기 UI 표시
        presenter.Initialize();

        Debug.Log("CharacterInfo 초기화 완료");
    }
}