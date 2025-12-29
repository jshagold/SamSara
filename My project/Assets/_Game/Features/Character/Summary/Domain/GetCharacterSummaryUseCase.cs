using System;
using System.Collections.Generic;

public class GetCharacterSummaryUseCase
{
    // TODO CharacterRepository 필요
    private readonly IDailyStateRepository _dailyStateRepo;

    // TODO CharacterRepository 필요
    public GetCharacterSummaryUseCase(IDailyStateRepository dailyStateRepo)
    {
        _dailyStateRepo = dailyStateRepo;
    }

    public List<MainSceneCharacterSummaryDto> GetCharacterSummaryList()
    {
        List<MainSceneCharacterSummaryDto> dataList = new List<MainSceneCharacterSummaryDto>();

        // TODO 캐릭터 데이터 가져오도록 수정해야함. 지금은 임시 데이터
        string charId = "character_main_0";
        //bool[] actionSlots = _dailyStateRepo.GetActionSlot(charId: charId);
        bool[] actionSlots = new bool[] { true, true, true };

        dataList.Add(new MainSceneCharacterSummaryDto(
            characterId: charId,
            currentHp: 1000,
            maxHp: 1000,
            actionFlags: actionSlots
        ));


        return dataList;
    }
}