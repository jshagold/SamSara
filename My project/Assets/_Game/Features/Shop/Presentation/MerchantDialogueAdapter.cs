using Samsara.Features.Event.MasterData;
using Samsara.Features.Shop.MasterData;

namespace Samsara.Features.Shop.Presentation
{
    /// <summary>
    /// MerchantDialogue[] → EventDialogue[] 변환 어댑터.
    /// EventDialogueOverlay UI를 상인 대사에 재사용하기 위한 정적 유틸리티.
    /// </summary>
    public static class MerchantDialogueAdapter
    {
        public static EventDialogue[] ToEventDialogues(MerchantDialogue[] merchantDialogues)
        {
            if (merchantDialogues == null || merchantDialogues.Length == 0)
                return new EventDialogue[0];

            var result = new EventDialogue[merchantDialogues.Length];
            for (int i = 0; i < merchantDialogues.Length; i++)
            {
                var md = merchantDialogues[i];
                // 상인 대화는 portrait 키를 사용하지 않는다 (MerchantSO.Portrait Sprite로 별도 표시).
                result[i] = new EventDialogue(
                    portraitSpriteKey: null,
                    dialogueText:      md.Text,
                    speakerName:       md.SpeakerName,
                    speakerPosition:   md.SpeakerPosition
                );
            }
            return result;
        }
    }
}
