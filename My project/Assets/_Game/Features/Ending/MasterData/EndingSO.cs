using UnityEngine;

namespace Samsara.Features.Ending.MasterData
{
    [CreateAssetMenu(fileName = "EndingSO", menuName = "Samsara/Ending/Ending")]
    public class EndingSO : ScriptableObject
    {
        private readonly string _logClass = $"[{nameof(EndingSO)}]";

        [SerializeField] private int              _id;
        [SerializeField] private EndingType       _endingType;
        [SerializeField] private bool             _isGameOver;
        [SerializeField] private string           _title;
        [SerializeField] private EndingDialogue[] _dialogues;
        [SerializeField] private string[]         _backgroundSpriteKeys;
        [SerializeField] private string           _resultText;
        [SerializeField] private string           _unlocksMainBgKey;
        [SerializeField] private string           _unlocksMainBgmKey;
        [SerializeField] private EndingCondition[] _conditions;
        [SerializeField] private int              _priority;

        public int               Id                   => _id;
        public EndingType        EndingType           => _endingType;
        public bool              IsGameOver           => _isGameOver;
        public string            Title                => _title;
        public EndingDialogue[]  Dialogues            => _dialogues;
        public string[]          BackgroundSpriteKeys => _backgroundSpriteKeys;
        public string            ResultText           => _resultText;
        public string            UnlocksMainBgKey     => _unlocksMainBgKey;
        public string            UnlocksMainBgmKey    => _unlocksMainBgmKey;
        /// <summary>발동 조건 배열 (AND). 빈 배열 = 조건 없음 = 폴백 후보.</summary>
        public EndingCondition[] Conditions           => _conditions;
        /// <summary>동일 EndingType 내 여러 EndingSO가 조건을 만족할 때 우선순위. 값이 높을수록 우선.</summary>
        public int               Priority             => _priority;
    }
}
