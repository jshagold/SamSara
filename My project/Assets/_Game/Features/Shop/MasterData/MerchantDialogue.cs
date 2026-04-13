using System;
using Samsara.Features.Event.MasterData;
using UnityEngine;

namespace Samsara.Features.Shop.MasterData
{
    [Serializable]
    public class MerchantDialogue
    {
        [SerializeField] private string         _speakerName;
        [SerializeField] private string         _text;
        [SerializeField] private SpeakerPosition _speakerPosition;

        public string          SpeakerName      => _speakerName;
        public string          Text             => _text;
        public SpeakerPosition SpeakerPosition  => _speakerPosition;
    }
}
