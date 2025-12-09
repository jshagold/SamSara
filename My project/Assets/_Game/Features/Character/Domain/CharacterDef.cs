using System;

// 캐릭터 기본 정의
[Serializable]
public class CharacterDef
{
	// 캐릭터 고유 ID
	public string characterId;

	// 캐릭터 이름
	public string displayName;

	// 초상화 리소스 키 (나중에 Sprite 로딩용)
	public string portraitRef;

	// 기본 스탯
	public Stats baseStats;
}