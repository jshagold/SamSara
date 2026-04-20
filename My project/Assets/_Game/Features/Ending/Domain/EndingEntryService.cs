using Cysharp.Threading.Tasks;
using Samsara.Core.Navigation;
using Samsara.Features.Character.Domain;
using Samsara.Features.Character.MasterData;
using Samsara.Features.Ending.MasterData;
using Samsara.Features.Stage.Domain;
using UnityEngine;

namespace Samsara.Features.Ending.Domain
{
    /// <summary>
    /// 엔딩 진입 공통 서비스. EndingCandidateSlot 기반으로 EndingId를 결정하고,
    /// 매칭 시 RunSummaryData를 구성한 뒤 PendingEndingContext를 설정하고 EndingScene으로 전환한다.
    /// 매칭 없으면 false를 반환해 호출자가 런을 계속 처리하도록 한다.
    /// Pure C# class. Constructor DI.
    /// </summary>
    public class EndingEntryService : IEndingEntryService
    {
        private readonly string _logClass = $"[{nameof(EndingEntryService)}]";

        private readonly IEndingResolver         _endingResolver;
        private readonly ICharacterRunRepository _characterRunRepo;
        private readonly IStageRepository        _stageRepo;
        private readonly EvolutionNodeSO[]       _evolutionNodes;
        private readonly ISceneNavigator         _sceneNavigator;
        private readonly GameContext             _gameContext;

        public EndingEntryService(
            IEndingResolver         endingResolver,
            ICharacterRunRepository characterRunRepo,
            IStageRepository        stageRepo,
            EvolutionNodeSO[]       evolutionNodes,
            ISceneNavigator         sceneNavigator,
            GameContext             gameContext)
        {
            _endingResolver   = endingResolver;
            _characterRunRepo = characterRunRepo;
            _stageRepo        = stageRepo;
            _evolutionNodes   = evolutionNodes;
            _sceneNavigator   = sceneNavigator;
            _gameContext      = gameContext;
        }

        // ──────────────────────────────────────────────
        // IEndingEntryService
        // ──────────────────────────────────────────────

        public async UniTask<bool> TryEnterEndingAsync(EndingCandidateSlot slot, EndingContext context)
        {
            int? endingId = _endingResolver.TryResolve(slot, context);
            if (!endingId.HasValue)
            {
                Debug.Log($"{_logClass} TryEnterEndingAsync: 매칭 없음 — 런 계속.");
                return false;
            }

            Debug.Log($"{_logClass} TryEnterEndingAsync: 엔딩 매칭 — endingId={endingId.Value}");
            await EnterEndingAsync(endingId.Value);
            return true;
        }

        // ──────────────────────────────────────────────
        // Helpers
        // ──────────────────────────────────────────────

        private async UniTask EnterEndingAsync(int endingId)
        {
            var runData = _characterRunRepo.RunData;
            var summary = new RunSummaryData
            {
                TotalDays          = runData.Day,
                FinalEvolutionName = FindEvolutionName(runData.EvolutionNodeId),
                StagesCleared      = _stageRepo.RunData.ClearedStageCount,
                FinalGold          = runData.Gold
            };

            _gameContext.PendingEndingContext = new PendingEndingContext
            {
                EndingId   = endingId,
                RunSummary = summary
            };

            await _sceneNavigator.NavigateToAsync(SceneKey.Ending);
        }

        private string FindEvolutionName(string evolutionNodeId)
        {
            foreach (var node in _evolutionNodes)
                if (node.NodeId == evolutionNodeId) return node.CharacterName;
            return "Unknown";
        }
    }
}
