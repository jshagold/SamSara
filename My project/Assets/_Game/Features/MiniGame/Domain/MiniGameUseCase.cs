using System;
using Cysharp.Threading.Tasks;
using Samsara.Features.Character.Domain;
using Samsara.Features.Character.MasterData;
using UnityEngine;

namespace Samsara.Features.MiniGame.Domain
{
    public class MiniGameUseCase
    {
        private readonly string _logClass = $"[{nameof(MiniGameUseCase)}]";

        private readonly ICharacterRunRepository _characterRunRepo;

        // 라운드 설정
        public const int RoundCount = 3;

        // 스탯 변화량 범위 (임시값 — 스탯 설계 확정 후 변경 예정)
        private const int SuccessDeltaMin  = 8;
        private const int SuccessDeltaMax  = 10;
        private const int MaintainDeltaMin = -2;
        private const int MaintainDeltaMax = 2;
        private const int FailDeltaMin     = -5;
        private const int FailDeltaMax     = -3;

        public MiniGameUseCase(ICharacterRunRepository characterRunRepo)
        {
            _characterRunRepo = characterRunRepo;
        }

        /// <summary>지정 StatType의 현재 값을 반환한다.</summary>
        public int GetCurrentStatValue(StatType statType)
        {
            var data = _characterRunRepo.RunData;
            return statType switch
            {
                StatType.Hp        => data.Hp,
                StatType.Strength  => data.Strength,
                StatType.Toughness => data.Toughness,
                StatType.Agility   => data.Agility,
                _ => throw new InvalidOperationException($"{_logClass} 알 수 없는 StatType: {statType}")
            };
        }

        /// <summary>판정에 따른 스탯 변화량(랜덤)을 계산한다.</summary>
        public int CalculateStatDelta(MiniGameVerdict verdict)
        {
            return verdict switch
            {
                MiniGameVerdict.Success  => UnityEngine.Random.Range(SuccessDeltaMin,  SuccessDeltaMax  + 1),
                MiniGameVerdict.Maintain => UnityEngine.Random.Range(MaintainDeltaMin, MaintainDeltaMax + 1),
                MiniGameVerdict.Fail     => UnityEngine.Random.Range(FailDeltaMin,     FailDeltaMax     + 1),
                _ => throw new InvalidOperationException($"{_logClass} 알 수 없는 MiniGameVerdict: {verdict}")
            };
        }

        /// <summary>행동력 -1 및 스탯 변화를 적용하고 즉시 저장한다. (Save-on-Action)</summary>
        public async UniTask ApplyResultAndSave(StatType statType, int delta)
        {
            var data = _characterRunRepo.RunData;
            data.ActionPoints -= 1;

            switch (statType)
            {
                case StatType.Hp:        data.Hp        += delta; break;
                case StatType.Strength:  data.Strength  += delta; break;
                case StatType.Toughness: data.Toughness += delta; break;
                case StatType.Agility:   data.Agility   += delta; break;
                default: throw new InvalidOperationException($"{_logClass} 알 수 없는 StatType: {statType}");
            }

            _characterRunRepo.MarkDirty();
            await _characterRunRepo.SaveDataAsync();

            Debug.Log($"{_logClass} 결과 저장 완료 — StatType={statType} delta={delta} AP={data.ActionPoints}");
        }
    }
}
