using System.Collections.Generic;
using Samsara.Features.Character.Domain;
using Samsara.Features.Character.MasterData;

namespace Samsara.Features.MaintenanceScene.Domain
{
    public class MaintenanceViewModel
    {
        public int ActionPoints;
        public int MaxActionPoints;
        public int Gold;
        public int CurrentHp;
        public int MaxHp;
        public Dictionary<StatType, int> Stats;
        public string CurrentEvolutionNodeId;
    }

    public class MaintenanceUseCase
    {
        private readonly string _logClass = $"[{nameof(MaintenanceUseCase)}]";

        private readonly ICharacterRunRepository _characterRunRepo;
        private readonly EventUseCase _eventUseCase;

        public MaintenanceUseCase(ICharacterRunRepository characterRunRepo, EventUseCase eventUseCase)
        {
            _characterRunRepo = characterRunRepo;
            _eventUseCase = eventUseCase;
        }

        public MaintenanceViewModel GetMaintenanceViewModel()
        {
            var runData = _characterRunRepo.RunData;

            return new MaintenanceViewModel
            {
                ActionPoints       = runData.ActionPoints,
                MaxActionPoints    = runData.MaxActionPoints,
                Gold               = runData.Gold,
                CurrentHp          = runData.Hp,
                MaxHp              = runData.MaxHp,
                Stats              = new Dictionary<StatType, int>
                {
                    { StatType.Hp,        runData.MaxHp       },
                    { StatType.Strength,  runData.Strength    },
                    { StatType.Toughness, runData.Toughness   },
                    { StatType.Agility,   runData.Agility     },
                },
                CurrentEvolutionNodeId = runData.EvolutionNodeId,
            };
        }

        public bool CanPerformAction()
        {
            return _characterRunRepo.RunData.ActionPoints >= 1;
        }

        /// <summary>
        /// 행동력 1 소모. 메모리만 변경 — Save 호출 금지.
        /// </summary>
        public void ConsumeActionPoint()
        {
            _characterRunRepo.RunData.ActionPoints -= 1;
        }

        /// <summary>
        /// TODO: Phase 5 — EventUseCase 통합 후 실제 상인 활성 조건 구현.
        /// </summary>
        public bool IsMerchantAvailable()
        {
            return false;
        }

        /// <summary>
        /// TODO: Phase 5 — 포션 구매 로직 구현 (골드 차감, 스탯 회복 등).
        /// </summary>
        public void PurchasePotion(StatType statType)
        {
            // Phase 5 stub
        }
    }
}
