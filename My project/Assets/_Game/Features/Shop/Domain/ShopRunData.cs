using System.Collections.Generic;

namespace Samsara.Features.Shop.Domain
{
    /// <summary>
    /// 런 중 상점 상태 데이터. 순수 데이터 클래스 — 로직 없음.
    /// Newtonsoft.Json 직렬화 대상.
    /// </summary>
    public class ShopRunData
    {
        public int                 ActiveMerchantId   = -1;
        public int                 MerchantAppearedDay = 0;
        public Dictionary<int, int> RemainingStock    = new Dictionary<int, int>();
    }
}
