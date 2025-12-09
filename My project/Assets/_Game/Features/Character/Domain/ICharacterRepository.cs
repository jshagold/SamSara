// 캐릭터 정의(기본 정보)를 읽어오는 저장소 인터페이스
public interface ICharacterDefRepository
{
    // 메인 캐릭터 하나로 가정
    CharacterDef GetMainCharacter();
}