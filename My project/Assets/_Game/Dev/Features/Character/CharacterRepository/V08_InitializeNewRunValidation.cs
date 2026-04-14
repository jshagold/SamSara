using System;
using Samsara.Features.Character.Data;
using UnityEngine;

namespace Samsara.Dev.Character
{
    /// <summary>
    /// [V-08] InitializeNewRun()이 CharacterRunData를 올바른 초기값으로 설정하는지 검증.
    /// Dev 전용. 프로덕션 빌드에 포함하지 말 것.
    /// 사용법: 빈 GameObject에 컴포넌트 추가 → Play Mode 진입.
    ///
    /// NOTE: D-01에 의해 InitializeNewRun()은 SaveDataAsync()를 내부 호출하지 않음.
    ///       파일 I/O가 전혀 없으므로 동기 검증 가능. 백업/복구 불필요.
    /// </summary>
    public class V08_InitializeNewRunValidation : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(V08_InitializeNewRunValidation)}]";

        private void Start()
        {
            RunValidation();
        }

        private void RunValidation()
        {
            var runConfig = Resources.Load<RunConfigSO>("MasterData/DefaultRunConfig");
            if (runConfig == null)
                throw new InvalidOperationException(
                    $"{_logClass} DefaultRunConfig.asset이 Resources/MasterData/에 없습니다.");

            Debug.Log($"{_logClass} === V-08 검증 시작: InitializeNewRun() 초기값 설정 ===");

            // ── InitializeNewRun 호출 (파일 I/O 없음 — D-01) ────────────
            var runRepo = new CharacterRunRepository();
            runRepo.InitializeNewRun(runConfig);

            var d = runRepo.RunData;

            string expectedNodeId = runConfig.DefaultEvolutionNodeId.ToString();

            // ── 필드별 검증 ──────────────────────────────────────────────
            bool nodePass  = d.EvolutionNodeId == expectedNodeId;
            bool dayPass   = d.Day             == runConfig.InitialDay;
            bool goldPass  = d.Gold            == runConfig.InitialGold;
            bool apPass    = d.ActionPoints    == runConfig.InitialActionPoints;
            bool maxApPass = d.MaxActionPoints == runConfig.InitialMaxActionPoints;

            Debug.Log($"{_logClass} --- [CharacterRunData 초기값 검증] ---");
            LogCheck($"EvolutionNodeId == \"{expectedNodeId}\"",
                     nodePass,  $"\"{d.EvolutionNodeId}\"");
            LogCheck($"Day == {runConfig.InitialDay}",
                     dayPass,   d.Day.ToString());
            LogCheck($"Gold == {runConfig.InitialGold}",
                     goldPass,  d.Gold.ToString());
            LogCheck($"ActionPoints == {runConfig.InitialActionPoints}",
                     apPass,    d.ActionPoints.ToString());
            LogCheck($"MaxActionPoints == {runConfig.InitialMaxActionPoints}",
                     maxApPass, d.MaxActionPoints.ToString());

            // ── 최종 판정 ────────────────────────────────────────────────
            bool allPass = nodePass && dayPass && goldPass && apPass && maxApPass;

            Debug.Log($"{_logClass} -------------------------------------------");
            if (allPass)
                Debug.Log($"{_logClass} [PASS] V-08: InitializeNewRun(RunConfigSO)이 모든 초기값을 올바르게 설정함.");
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
