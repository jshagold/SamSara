using System;

namespace Samsara.Features.Ending.MasterData
{
    /// <summary>
    /// 엔딩의 성격·콘텐츠 분류 태그 ([Flags]).
    /// 로직 매칭에는 사용하지 않음 — 기획/UI/코덱스 표시 전용.
    /// </summary>
    [Flags]
    public enum EndingType
    {
        None = 0,
        Good = 1,
        Bad  = 2
    }
}
