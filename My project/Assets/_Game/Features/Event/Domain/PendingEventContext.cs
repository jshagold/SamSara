using Samsara.Core.Navigation;

namespace Samsara.Features.Event.Domain
{
    /// <summary>
    /// 씬 간 데이터 전달 — EventScene 진입 컨텍스트.
    /// EventScene 진입 전 설정, 진입 후 소비.
    /// </summary>
    public class PendingEventContext
    {
        public int EventId { get; set; }
        public SceneKey ReturnScene { get; set; }
        public string BackgroundSpriteKey { get; set; }
        public bool IsReturningFromBattle { get; set; }

        /// <summary>이벤트 완료 여부. Stage 복귀 시 노드 진행 처리에 사용.</summary>
        public bool IsCompleted { get; set; }
    }
}
