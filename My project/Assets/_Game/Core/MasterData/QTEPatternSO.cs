using System;
using UnityEngine;

namespace Samsara.Core.MasterData
{
    [CreateAssetMenu(fileName = "QTEPatternSO", menuName = "Samsara/Core/QTEPattern")]
    public class QTEPatternSO : ScriptableObject
    {
        private readonly string _logClass = $"[{nameof(QTEPatternSO)}]";

        [SerializeField] private int _patternId;
        [SerializeField] private QTEData[] _qteDataList;

        public int PatternId => _patternId;
        public QTEData[] QteDataList => _qteDataList;
    }

    [Serializable]
    public class QTEData
    {
        [SerializeField] private string _spriteKey;
        [SerializeField] private Vector2 _coordinate;  // ratio value (0-1)
        [SerializeField] private float _duration;
        [SerializeField] private float _intervalToNext;

        public string SpriteKey => _spriteKey;
        public Vector2 Coordinate => _coordinate;
        public float Duration => _duration;
        public float IntervalToNext => _intervalToNext;
    }
}
