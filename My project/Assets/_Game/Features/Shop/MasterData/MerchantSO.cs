using UnityEngine;  // ScriptableObject, SerializeField, CreateAssetMenu

namespace Samsara.Features.Shop.MasterData
{
    [CreateAssetMenu(fileName = "MerchantSO", menuName = "Samsara/Shop/Merchant")]
    public class MerchantSO : ScriptableObject
    {
        private readonly string _logClass = $"[{nameof(MerchantSO)}]";

        [SerializeField] private int                _id;
        [SerializeField] private string             _merchantName;
        [SerializeField] private string             _portraitKey;
        [SerializeField] private string             _shopSpriteKey;
        [SerializeField] private int                _stayDuration;
        [SerializeField] private MerchantSaleItem[] _saleItems;
        [SerializeField] private MerchantDialogue[] _greetingDialogues;
        [SerializeField] private MerchantDialogue[] _farewellDialogues;

        public int                Id                 => _id;
        public string             MerchantName       => _merchantName;
        public string             PortraitKey        => _portraitKey;
        public string             ShopSpriteKey      => _shopSpriteKey;
        public int                StayDuration       => _stayDuration;
        public MerchantSaleItem[] SaleItems          => _saleItems;
        public MerchantDialogue[] GreetingDialogues  => _greetingDialogues;
        public MerchantDialogue[] FarewellDialogues  => _farewellDialogues;
    }
}
