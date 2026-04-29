using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Samsara.Core.Navigation;
using Samsara.Core.Tree;
using Samsara.Features.Character.Domain;
using Samsara.Features.Character.MasterData;

namespace Samsara.Features.ReplayScene.Domain
{
    public class ReplayUseCase : IReplayUseCase
    {
        private readonly string _logClass = $"[{nameof(ReplayUseCase)}]";

        private readonly ICharacterAccountRepository _accountRepo;
        private readonly EvolutionNodeSO[]           _evolutionNodes;
        private readonly IRestartFlow                _restartFlow;
        private readonly IPostRestartSceneRouter     _sceneRouter;
        private readonly ISceneNavigator             _sceneNavigator;
        private readonly PendingReplayContext        _pendingContext;

        public ReplayUseCase(
            ICharacterAccountRepository accountRepo,
            EvolutionNodeSO[]           evolutionNodes,
            IRestartFlow                restartFlow,
            IPostRestartSceneRouter     sceneRouter,
            ISceneNavigator             sceneNavigator,
            PendingReplayContext        pendingContext)
        {
            _accountRepo    = accountRepo;
            _evolutionNodes = evolutionNodes;
            _restartFlow    = restartFlow;
            _sceneRouter    = sceneRouter;
            _sceneNavigator = sceneNavigator;
            _pendingContext = pendingContext;
        }

        public IReadOnlyList<EvolutionNodeSO> GetAllNodes()
        {
            return _evolutionNodes;
        }

        public ReplayNodeState ClassifyNode(EvolutionNodeSO node)
        {
            if (node == null)
                throw new InvalidOperationException($"{_logClass} ClassifyNode: node is null");

            if (_accountRepo.AccountData.UnlockedEvolutionNodeIds.Contains(node.NodeId))
                return ReplayNodeState.Selectable;

            if (node.IsHidden)
                return ReplayNodeState.Hidden;

            return ReplayNodeState.Locked;
        }

        public async UniTask ExecuteRestartAsync(string selectedNodeId, CancellationToken ct = default)
        {
            if (string.IsNullOrEmpty(selectedNodeId))
                throw new InvalidOperationException($"{_logClass} ExecuteRestartAsync: selectedNodeId is null or empty");

            await _restartFlow.ExecuteAsync(selectedNodeId, _pendingContext, ct);

            var nextSceneKey = _sceneRouter.ResolveNextSceneKey(selectedNodeId, _pendingContext);

            await _sceneNavigator.NavigateToAsync(nextSceneKey, ct);
        }
    }
}
