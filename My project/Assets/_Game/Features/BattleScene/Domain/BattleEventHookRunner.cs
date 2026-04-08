using Cysharp.Threading.Tasks;

namespace Samsara.Features.BattleScene.Domain
{
    /// <summary>
    /// 배틀 이벤트 훅 실행기. 특정 훅 포인트에서 매칭되는 BattleEventData를 실행한다.
    /// Phase 1: 매칭 검사만 수행, 실제 이벤트 로직은 즉시 반환.
    /// </summary>
    public class BattleEventHookRunner
    {
        private readonly string _logClass = $"[{nameof(BattleEventHookRunner)}]";

        private readonly BattleEventData[] _events;

        public BattleEventHookRunner(BattleEventData[] events)
        {
            _events = events;
        }

        /// <summary>
        /// 해당 훅 타입에 매칭되는 이벤트가 있으면 실행한다.
        /// Phase 1: 매칭 확인 후 즉시 반환.
        /// </summary>
        public async UniTask CheckHook(BattleHookType hookType, BattleRuntimeData data)
        {
            if (_events == null) return;

            bool hasMatch = false;
            for (int i = 0; i < _events.Length; i++)
            {
                if (_events[i].HookType == hookType)
                {
                    hasMatch = true;
                    break;
                }
            }

            if (!hasMatch) return;

            // Phase 1: always returns immediately
            await UniTask.CompletedTask;
        }
    }
}
