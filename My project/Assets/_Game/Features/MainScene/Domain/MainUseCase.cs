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
                MaxHp = runData.MaxHp,
                ActionPoints = runData.ActionPoints,
                MaxActionPoints = runData.MaxActionPoints,
                CurrentEvolutionNodeId = runData.EvolutionNodeId,
            };
        }
    }
}
