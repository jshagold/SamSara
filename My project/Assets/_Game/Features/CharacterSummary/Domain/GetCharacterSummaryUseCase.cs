public class GetCharacterSummaryUseCase
{
    // TODO CharacterRepository 필요
    private readonly IDailyStateRepository _dailyStateRepo;

    // TODO CharacterRepository 필요
    public GetCharacterSummaryUseCase(IDailyStateRepository dailyStateRepo)
    {
        _dailyStateRepo = dailyStateRepo;
    }

    public MainCharacterSummaryDto GetDto()
    {
        return new MainCharacterSummaryDto
        {
            // TODO 캐릭터 정보 받아와야함
            CharacterId = "",
            CurrentHp = 0,
            MaxHp = 0,
            ActionFlags = _dailyStateRepo.GetActionSlot("")
        };
    }
}