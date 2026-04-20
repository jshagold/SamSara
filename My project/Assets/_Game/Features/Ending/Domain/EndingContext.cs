using Samsara.Features.BattleScene.Domain;
using Samsara.Features.Event.MasterData;

namespace Samsara.Features.Ending.Domain
{
    /// <summary>
    /// 엔딩 Resolver에 전달하는 런타임 트리거 정보.
    /// 트리거 종류에 따라 사용하는 필드가 달라진다.
    /// </summary>
    public struct EndingContext
    {
        /// <summary>BattleVictory / BattleDefeat 트리거 시 사용.</summary>
        public BattleResult? BattleResult;

        /// <summary>EventResult 트리거 시 사용 — 발동 이벤트 ID.</summary>
        public int? EventId;

        /// <summary>EventResult 트리거 시 사용 — 이벤트 결과 타입.</summary>
        public EventResultType? EventResultType;
    }
}
