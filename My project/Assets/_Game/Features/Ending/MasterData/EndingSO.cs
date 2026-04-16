using UnityEngine;

namespace Samsara.Features.Ending.MasterData
{
    [CreateAssetMenu(fileName = "EndingSO", menuName = "Samsara/Ending/Ending")]
    public class EndingSO : ScriptableObject
    {
        private readonly string _logClass = $"[{nameof(EndingSO)}]";

        [SerializeField] private int                _id;
        /// <summary>이 엔딩이 발동되는 트리거 시점.</summary>
        [SerializeField] private EndingTriggerKind  _triggerKind;
        /// <summary>엔딩 성격 분류 태그 ([Flags]). 기획/UI/코덱스 전용 — 매칭 로직에 미사용.</summary>
        [SerializeField] private EndingType         _categories;
        [SerializeField] private bool               _isGameOver;
        [SerializeField] private string             _title;
        [SerializeField] private EndingDialogue[]   _dialogues;
        [SerializeField] private string[]           _backgroundSpriteKeys;
        [SerializeField] private string             _resultText;
        [SerializeField] private string             _unlocksMainBgKey;
        [SerializeField] private string             _unlocksMainBgmKey;
        /// <summary>발동 조건 배열 (AND). 빈 배열 = 조건 없음 = 폴백 후보.</summary>
        [SerializeField] private EndingCondition[]  _conditions;
        /// <summary>동일 TriggerKind 내 여러 EndingSO가 조건을 만족할 때 우선순위. 값이 높을수록 우선.</summary>
        [SerializeField] private int                _priority;

        public int               Id                   => _id;
        public EndingTriggerKind TriggerKind          => _triggerKind;
        public EndingType        Categories           => _categories;
        public bool              IsGameOver           => _isGameOver;
        public string            Title                => _title;
        public EndingDialogue[]  Dialogues            => _dialogues;
        public string[]          BackgroundSpriteKeys => _backgroundSpriteKeys;
        public string            ResultText           => _resultText;
        public string            UnlocksMainBgKey     => _unlocksMainBgKey;
        public string            UnlocksMainBgmKey    => _unlocksMainBgmKey;
        public EndingCondition[] Conditions           => _conditions;
        public int               Priority             => _priority;
    }
}
