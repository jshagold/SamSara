public class MainSceneCharacterSummaryDto
{
    public string CharacterId;

    public float CurrentHp;
    public float MaxHp;

    public bool[] ActionFlags;

    public MainSceneCharacterSummaryDto(string characterId, float currentHp, float maxHp, bool[] actionFlags)
    {
        CharacterId = characterId;
        CurrentHp = currentHp;
        MaxHp = maxHp;
        ActionFlags = actionFlags;
    }
}