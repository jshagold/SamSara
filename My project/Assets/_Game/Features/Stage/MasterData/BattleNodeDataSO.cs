using System;
using Samsara.Features.Ending.MasterData;
using UnityEngine;

namespace Samsara.Features.Stage.MasterData
{
    [CreateAssetMenu(fileName = "BattleNodeDataSO", menuName = "Samsara/Stage/BattleNodeData")]
    public class BattleNodeDataSO : ScriptableObject
    {
        private readonly string _logClass = $"[{nameof(BattleNodeDataSO)}]";

        [SerializeField] private EnemySpawn[]        _enemySpawns;
        [SerializeField] private EndingCandidateSlot _victoryEndings;
        [SerializeField] private EndingCandidateSlot _defeatEndings;

        public EnemySpawn[]        EnemySpawns     => _enemySpawns;
        public EndingCandidateSlot VictoryEndings  => _victoryEndings;
        public EndingCandidateSlot DefeatEndings   => _defeatEndings;
    }

    [Serializable]
    public class EnemySpawn
    {
        [SerializeField] private int _enemyId;
        [SerializeField] private int _count;

        public int EnemyId => _enemyId;
        public int Count => _count;
    }
}
