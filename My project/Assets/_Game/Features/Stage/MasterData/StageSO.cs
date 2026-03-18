using System;
using UnityEngine;

namespace Samsara.Features.Stage.MasterData
{
    [CreateAssetMenu(fileName = "StageSO", menuName = "Samsara/Stage/Stage")]
    public class StageSO : ScriptableObject
    {
        private readonly string _logClass = $"[{nameof(StageSO)}]";

        [SerializeField] private string _stageId;
        [SerializeField] private string _stageName;
        [SerializeField] private string _backgroundSpriteKey;
        [SerializeField] private string _backgroundMusicKey;
        [SerializeField] private bool _isFixed;
        [SerializeField] private StageNodeSO[] _fixedNodes;
        [SerializeField] private StageNodePool _randomNodePool;

        public string StageId => _stageId;
        public string StageName => _stageName;
        public string BackgroundSpriteKey => _backgroundSpriteKey;
        public string BackgroundMusicKey => _backgroundMusicKey;
        public bool IsFixed => _isFixed;
        public StageNodeSO[] FixedNodes => _fixedNodes;
        public StageNodePool RandomNodePool => _randomNodePool;
    }

    [Serializable]
    public class StageNodePool
    {
        [SerializeField] private int _minBattleNodes;
        [SerializeField] private int _maxBattleNodes;
        [SerializeField] private int _minEventNodes;
        [SerializeField] private int _maxEventNodes;
        [SerializeField] private int _bossNodeCount;

        public int MinBattleNodes => _minBattleNodes;
        public int MaxBattleNodes => _maxBattleNodes;
        public int MinEventNodes => _minEventNodes;
        public int MaxEventNodes => _maxEventNodes;
        public int BossNodeCount => _bossNodeCount;
    }
}
