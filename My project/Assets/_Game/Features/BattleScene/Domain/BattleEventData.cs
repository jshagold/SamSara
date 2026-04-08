namespace Samsara.Features.BattleScene.Domain
{
    /// <summary>
    /// 배틀 이벤트 훅 데이터. 어떤 훅 타입에서, 어떤 조건으로, 어떤 이벤트를 트리거할지 정의.
    /// </summary>
    public class BattleEventData
    {
        private readonly string _logClass = $"[{nameof(BattleEventData)}]";

        public BattleHookType HookType { get; }
        public string TriggerCondition { get; }  // future use
        public string EventReference { get; }    // future event data ref

        public BattleEventData(BattleHookType hookType, string triggerCondition, string eventReference)
        {
            HookType = hookType;
            TriggerCondition = triggerCondition;
            EventReference = eventReference;
        }
    }
}
