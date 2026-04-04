using System;
using System.Collections.Generic;
using Samsara.Core.MasterData;
using Samsara.Features.Character.MasterData;
using Samsara.Features.Stage.Domain;
using Samsara.Features.Stage.MasterData;
using UnityEngine;

namespace Samsara.Features.Stage.Data
{
    public class StageMasterDataRepository : IStageMasterDataRepository
    {
        private readonly string _logClass = $"[{nameof(StageMasterDataRepository)}]";

        private Dictionary<string, StageSO>          _stageCache;
        private Dictionary<string, StageNodeSO>      _nodeCache;
        private Dictionary<int, EnemySO>             _enemyCache;
        private Dictionary<string, EvolutionNodeSO>  _evolutionNodeCache;
        private List<StageSO>                        _allStages;

        // ──────────────────────────────────────────────
        // Initialize — 메인 스레드 전용 (Resources API)
        // ──────────────────────────────────────────────

        public void Initialize()
        {
            _stageCache         = new Dictionary<string, StageSO>();
            _nodeCache          = new Dictionary<string, StageNodeSO>();
            _enemyCache         = new Dictionary<int, EnemySO>();
            _evolutionNodeCache = new Dictionary<string, EvolutionNodeSO>();

            var stages = Resources.LoadAll<StageSO>("MasterData");
            foreach (var stage in stages)
                _stageCache[stage.StageId] = stage;

            var nodes = Resources.LoadAll<StageNodeSO>("MasterData");
            foreach (var node in nodes)
                _nodeCache[node.NodeId] = node;

            var enemies = Resources.LoadAll<EnemySO>("MasterData");
            foreach (var enemy in enemies)
                _enemyCache[enemy.EnemyId] = enemy;

            var evolutionNodes = Resources.LoadAll<EvolutionNodeSO>("MasterData");
            foreach (var evoNode in evolutionNodes)
                _evolutionNodeCache[evoNode.NodeId] = evoNode;

            _allStages = new List<StageSO>(_stageCache.Values);

            Debug.Log($"{_logClass} Initialize 완료 — Stage:{_stageCache.Count}, Node:{_nodeCache.Count}, Enemy:{_enemyCache.Count}, EvolutionNode:{_evolutionNodeCache.Count}");
        }

        // ──────────────────────────────────────────────
        // Accessors
        // ──────────────────────────────────────────────

        public StageSO GetStageById(string id)
        {
            if (_stageCache.TryGetValue(id, out var stage)) return stage;
            throw new InvalidOperationException($"{_logClass} StageSO not found: {id}");
        }

        public StageNodeSO GetNodeById(string id)
        {
            if (_nodeCache.TryGetValue(id, out var node)) return node;
            throw new InvalidOperationException($"{_logClass} StageNodeSO not found: {id}");
        }

        public IReadOnlyList<StageSO> GetAllStages() => _allStages;

        public EnemySO GetEnemyById(int enemyId)
        {
            if (_enemyCache.TryGetValue(enemyId, out var enemy)) return enemy;
            throw new InvalidOperationException($"{_logClass} EnemySO not found: {enemyId}");
        }

        public EvolutionNodeSO GetEvolutionNodeById(string nodeId)
        {
            if (_evolutionNodeCache.TryGetValue(nodeId, out var node)) return node;
            throw new InvalidOperationException($"{_logClass} EvolutionNodeSO not found: {nodeId}");
        }
    }
}
