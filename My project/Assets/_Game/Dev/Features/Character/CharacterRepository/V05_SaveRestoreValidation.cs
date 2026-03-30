using System;
using System.IO;
using Cysharp.Threading.Tasks;
using Samsara.Features.Character.Data;
using Samsara.Features.Character.MasterData;
using UnityEngine;

namespace Samsara.Dev.Character
{
    /// <summary>
    /// [V-05] SaveDataAsync() 후 다음 실행에서 값이 올바르게 복원되는지 검증.
    /// Dev 전용. 프로덕션 빌드에 포함하지 말 것.
    /// 사용법: 빈 GameObject에 컴포넌트 추가 → Inspector에서 _testStats 할당 → Play Mode 진입.
    /// </summary>
    public class V05_SaveRestoreValidation : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(V05_SaveRestoreValidation)}]";

        [SerializeField] private CharacterStatsSO _testStats;

        // ── 검증에 사용할 고정 테스트 값 ──────────────────────────────
        private const string TestStartNodeId   = "v05_start_node";
        private const string TestUnlockedNodeId = "v05_unlocked_node";
        private const string TestCodexId        = "v05_codex_entry";

        private void Start()
        {
            RunValidationAsync().Forget();
        }

        private async UniTask RunValidationAsync()
        {
            if (_testStats == null)
                throw new InvalidOperationException($"{_logClass} _testStats가 Inspector에서 할당되지 않았습니다.");

            Debug.Log($"{_logClass} === V-05 검증 시작: SaveDataAsync() 후 값 복원 ===");

            var runSavePath     = Application.persistentDataPath + "/run_save.json";
            var accountSavePath = Application.persistentDataPath + "/account_save.json";
            var runBackupPath     = runSavePath     + ".v05.bak";
            var accountBackupPath = accountSavePath + ".v05.bak";

            // ── 기존 저장 파일 백업 ───────────────────────────────────────
            bool runFileExisted     = File.Exists(runSavePath);
            bool accountFileExisted = File.Exists(accountSavePath);

            if (runFileExisted)     File.Move(runSavePath,     runBackupPath);
            if (accountFileExisted) File.Move(accountSavePath, accountBackupPath);

            try
            {
                // ════════════════════════════════════════════════════════════
                // Phase 1 — 테스트 값 기록 (저장)
                // ════════════════════════════════════════════════════════════

                // RunRepository: InitializeNewRun → SaveDataAsync
                var runRepoWriter = new CharacterRunRepository();
                runRepoWriter.InitializeNewRun(TestStartNodeId, _testStats);
                // InitializeNewRun 내부의 Forget() 저장과 경합 없이 완료를 보장.
                // _isDirty=false 는 File.WriteAllText 완료 이후 설정되므로,
                // early-return 했다면 파일은 이미 디스크에 존재함.
                await runRepoWriter.SaveDataAsync();

                // AccountRepository: Load → Unlock/Codex → SaveDataAsync
                var accountRepoWriter = new CharacterAccountRepository();
                await accountRepoWriter.LoadDataAsync();
                accountRepoWriter.UnlockEvolutionNode(TestStartNodeId);
                accountRepoWriter.UnlockEvolutionNode(TestUnlockedNodeId);
                accountRepoWriter.RegisterCodex(TestCodexId);
                await accountRepoWriter.SaveDataAsync();

                Debug.Log($"{_logClass} Phase 1 완료: 테스트 값 저장됨.");

                // ════════════════════════════════════════════════════════════
                // Phase 2 — 새 인스턴스로 로드 (다음 실행 시뮬레이션)
                // ════════════════════════════════════════════════════════════
                var runRepoReader     = new CharacterRunRepository();
                var accountRepoReader = new CharacterAccountRepository();

                await UniTask.WhenAll(
                    runRepoReader.LoadDataAsync(),
                    accountRepoReader.LoadDataAsync()
                );

                Debug.Log($"{_logClass} Phase 2 완료: 새 인스턴스에서 로드됨.");

                // ════════════════════════════════════════════════════════════
                // Phase 3 — 복원 값 검증
                // ════════════════════════════════════════════════════════════
                var runData     = runRepoReader.RunData;
                var accountData = accountRepoReader.AccountData;

                // ── CharacterRunData 검증 ────────────────────────────────
                Debug.Log($"{_logClass} --- [CharacterRunData 복원 검증] ---");

                bool runNotNull = runData != null;
                LogCheck("RunData != null",                      runNotNull);
                LogCheck($"Hp == {_testStats.Hp}",               runNotNull && runData.Hp         == _testStats.Hp,
                                                                  runNotNull ? $"실제={runData.Hp}"          : "null");
                LogCheck($"Strength == {_testStats.Strength}",   runNotNull && runData.Strength   == _testStats.Strength,
                                                                  runNotNull ? $"실제={runData.Strength}"    : "null");
                LogCheck($"Toughness == {_testStats.Toughness}", runNotNull && runData.Toughness  == _testStats.Toughness,
                                                                  runNotNull ? $"실제={runData.Toughness}"   : "null");
                LogCheck($"Agility == {_testStats.Agility}",       runNotNull && runData.Agility      == _testStats.Agility,
                                                                  runNotNull ? $"실제={runData.Agility}"       : "null");
                LogCheck($"EvolutionNodeId == \"{TestStartNodeId}\"",
                                                                  runNotNull && runData.EvolutionNodeId == TestStartNodeId,
                                                                  runNotNull ? $"실제=\"{runData.EvolutionNodeId}\"" : "null");
                LogCheck("Day == 1",                             runNotNull && runData.Day  == 1,
                                                                  runNotNull ? $"실제={runData.Day}"         : "null");
                LogCheck("Gold == 0",                            runNotNull && runData.Gold == 0,
                                                                  runNotNull ? $"실제={runData.Gold}"        : "null");

                bool runPass = runNotNull
                    && runData.Hp              == _testStats.Hp
                    && runData.Strength        == _testStats.Strength
                    && runData.Toughness       == _testStats.Toughness
                    && runData.Agility           == _testStats.Agility
                    && runData.EvolutionNodeId == TestStartNodeId
                    && runData.Day             == 1
                    && runData.Gold            == 0;

                // ── CharacterAccountData 검증 ────────────────────────────
                Debug.Log($"{_logClass} --- [CharacterAccountData 복원 검증] ---");

                bool accNotNull = accountData != null;
                LogCheck("AccountData != null", accNotNull);

                bool accHasStartNode    = accNotNull && accountData.UnlockedEvolutionNodeIds.Contains(TestStartNodeId);
                bool accHasUnlockNode   = accNotNull && accountData.UnlockedEvolutionNodeIds.Contains(TestUnlockedNodeId);
                bool accHasCodex        = accNotNull && accountData.CompletedCodexIds.Contains(TestCodexId);

                LogCheck($"UnlockedEvolutionNodeIds.Contains(\"{TestStartNodeId}\")",   accHasStartNode);
                LogCheck($"UnlockedEvolutionNodeIds.Contains(\"{TestUnlockedNodeId}\")", accHasUnlockNode);
                LogCheck($"CompletedCodexIds.Contains(\"{TestCodexId}\")",              accHasCodex);

                bool accountPass = accNotNull && accHasStartNode && accHasUnlockNode && accHasCodex;

                // ── 최종 판정 ────────────────────────────────────────────
                Debug.Log($"{_logClass} -------------------------------------------");
                if (runPass && accountPass)
                    Debug.Log($"{_logClass} [PASS] V-05: SaveDataAsync() 후 모든 값이 정확히 복원됨.");
                else
                    Debug.LogError($"{_logClass} [FAIL] V-05: 복원 실패 항목 있음. 위 로그를 확인하세요.");
            }
            finally
            {
                // ── 테스트 파일 제거 & 기존 파일 복구 ────────────────────
                if (File.Exists(Application.persistentDataPath + "/run_save.json"))
                    File.Delete(Application.persistentDataPath + "/run_save.json");
                if (File.Exists(Application.persistentDataPath + "/account_save.json"))
                    File.Delete(Application.persistentDataPath + "/account_save.json");

                if (runFileExisted)     File.Move(runBackupPath,     Application.persistentDataPath + "/run_save.json");
                if (accountFileExisted) File.Move(accountBackupPath, Application.persistentDataPath + "/account_save.json");

                Debug.Log($"{_logClass} === V-05 검증 종료. 기존 저장 파일 복구 완료. ===");
            }
        }

        private void LogCheck(string label, bool pass, string actual = null)
        {
            if (pass)
                Debug.Log($"{_logClass}   [PASS] {label}");
            else
                Debug.LogError($"{_logClass}   [FAIL] {label}{(actual != null ? $" ({actual})" : "")}");
        }
    }
}
