using System;
using UnityEngine;

namespace Samsara.Features.Ending.MasterData
{
    [Serializable]
    public class EndingDialogue
    {
        [SerializeField] private string _speakerName;
        [SerializeField] private string _speakerPortraitKey;
        [SerializeField] private string _text;
        [SerializeField] private int    _backgroundIndex = -1;

        public string SpeakerName         => _speakerName;
        public string SpeakerPortraitKey  => _speakerPortraitKey;
        public string Text                => _text;
        /// <summary>
        /// BackgroundSpriteKeys 배열의 인덱스. -1이면 배경 전환 없음.
        /// </summary>
        public int    BackgroundIndex     => _backgroundIndex;
    }
}
