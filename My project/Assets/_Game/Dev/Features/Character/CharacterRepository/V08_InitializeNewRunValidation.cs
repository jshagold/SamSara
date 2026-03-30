using System;
using Samsara.Features.Character.Data;
using Samsara.Features.Character.MasterData;
using UnityEngine;

namespace Samsara.Dev.Character
{
    /// <summary>
    /// [V-08] InitializeNewRun()이 CharacterRunData를 올바른 초기값으로 설정하는지 검증.
    /// Dev 전용. 프로덕션 빌드에 포함하지 말 것.
    /// 사용법: 빈 GameObject에 컴포넌트 추가 → Inspector에서 _testStats 할당 → Play Mode 진입.
    ///
    /// NOTE: D-01에 의해 InitializeNewRun()은 SaveDataAsync()를 내부 호출하지 않음.
    ///       파일 I/O가 전혀 없으므로 동기 검증 가능. 백업/복구 불필요.
    /// </summary>
    public class V08_InitializeNewRunValidation : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(V08_InitializeNewRunValidation)}]";

        [SerializeField] private CharacterStatsSO _testStats;

        private const string TestNodeId = "v08_start_node";

        private void Start()
        {
            RunValidation();
        }

        private void RunValidation()
        {
            if (_testStats == null)
                throw new InvalidOperationException(
                    $"{_logClass} _testStats가 Inspector에서 할당되지 않았습니다.");

            Debug.Log($"{_logClass} === V-08 검증 시작: InitializeNewRun() 초기값 설정 " +
                      $"(SO: {_testStats.name}) ===");

            // ── InitializeNewRun 호출 (파일 I/O 없음 — D-01) ────────────
            var runRepo = new CharacterRunRepository();
            runRepo.InitializeNewRun(TestNodeId, _testStats);

            var d = runRepo.RunData;

            // ── 필드별 검증 ──────────────────────────────────────────────
            bool hpPass    = d.Hp              == _testStats.Hp;
            bool strPass   = d.Strength        == _testStats.Strength;
            bool toughPass = d.Toughness       == _testStats.Toughness;
            bool speedPass = d.Agility           == _testStats.Agility;
            bool nodePass  = d.EvolutionNodeId == TestNodeId;
            bool dayPass   = d.Day             == 1;
            bool goldPass  = d.Gold            == 0;

            Debug.Log($"{_logClass} --- [CharacterRunData 초기값 검증] ---");
            LogCheck($"Hp == {_testStats.Hp}",                                          hpPass,    d.Hp.ToString());
            LogCheck($"Strength == {_testStats.Strength}",                              strPass,   d.Strength.ToString());
            LogCheck($"Toughness == {_testStats.Toughness}",                           toughPass, d.Toughness.ToString());
            LogCheck($"Agility == {_testStats.Agility}",                                  speedPass, d.Agility.ToString());
            LogCheck($"EvolutionNodeId == \"{TestNodeId}\"",                           nodePass,  $"\"{d.EvolutionNodeId}\"");
            LogCheck("Day == 1 (항상 1로 시작)",                                        dayPass,   d.Day.ToString());
            LogCheck("Gold == 0 (항상 0으로 시작)",                                     goldPass,  d.Gold.ToString());

            // ── 최종 판정 ────────────────────────────────────────────────
            bool allPass = hpPass && strPass && toughPass && speedPass
                        && nodePass && dayPass && goldPass;

            Debug.Log($"{_logClass} -------------------------------------------");
            if (allPass)
                Debug.Log($"{_logClass} [PASS] V-08: InitializeNewRun()이 모든 초기값을 올바르게 설정함.");
            else
                Debug.LogError($"{_logClass} [FAIL] V-08: 초기값 불일치 항목 있음. 위 로그를 확인하세요.");

            Debug.Log($"{_logClass} === V-08 검증 종료. ===");
        }

        private void LogCheck(string label, bool pass, string actual)
        {
            if (pass)
                Debug.Log($"{_logClass}   [PASS] {label}");
            else
                Debug.LogError($"{_logClass}   [FAIL] {label} (실제={actual})");
        }
    }
}
