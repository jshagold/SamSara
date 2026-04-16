namespace Samsara.Features.Event.Domain
{
    /// <summary>
    /// 이벤트가 발생한 경로 종류.
    /// </summary>
    public enum EventOriginKind
    {
        StageNode,               // 스테이지 노드에서 발생
        MaintenanceExploration   // 정비 씬 탐색에서 발생
    }
}
