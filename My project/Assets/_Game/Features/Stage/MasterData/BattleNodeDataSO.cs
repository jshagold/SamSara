using System;
using UnityEngine;

namespace Samsara.Features.Stage.MasterData
{
    [CreateAssetMenu(fileName = "BattleNodeDataSO", menuName = "Samsara/Stage/BattleNodeData")]
    public class BattleNodeDataSO : ScriptableObject
    {
        private readonly string _logClass = $"[{nameof(BattleNodeDataSO)}]";

        [SerializeField] private EnemySpawn[] _enemySpawns;

        public EnemySpawn[] EnemySpawns => _enemySpawns;
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
