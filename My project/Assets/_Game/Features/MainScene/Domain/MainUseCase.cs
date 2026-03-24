using Samsara.Features.Character.Domain;

namespace Samsara.Features.MainScene.Domain
{
    public class MainViewModel
    {
        public int Day;
        public int Gold;
        public int CurrentHp;
        public int MaxHp;
        public int ActionPoints;
        public int MaxActionPoints;
        public string CurrentEvolutionNodeId;
        public bool IsMerchantActive;
    }

    public class MainUseCase
    {
        private readonly string _logClass = $"[{nameof(MainUseCase)}]";

        private readonly ICharacterRunRepository _characterRunRepo;

        public MainUseCase(ICharacterRunRepository characterRunRepo)
        {
            _characterRunRepo = characterRunRepo;
        }

        public MainViewModel GetMainViewModel()
        {
            var runData = _characterRunRepo.RunData;

            return new MainViewModel
            {
                Day = runData.Day,
                Gold = runData.Gold,
                CurrentHp = runData.Hp,
                MaxHp = runData.Hp, // TODO: MaxHp 필드가 RunData에 추가되면 교체
                ActionPoints = runData.ActionPoints,
                MaxActionPoints = runData.MaxActionPoints,
                CurrentEvolutionNodeId = runData.EvolutionNodeId,
                // OQ-02 미해결: Merchant 활성 조건이 아직 정의되지 않음. v1에서는 항상 false.
                IsMerchantActive = false
            };
        }
    }
}
