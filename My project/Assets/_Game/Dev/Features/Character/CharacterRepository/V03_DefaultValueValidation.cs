using System.IO;
using Cysharp.Threading.Tasks;
using Samsara.Features.Character.Data;
using Samsara.Features.Character.MasterData;
using UnityEngine;

namespace Samsara.Dev
{
    /// <summary>
    /// [V-03] LoadDataAsync() — 저장 파일 없을 때 기본값 초기화 검증.
    /// Dev 전용. 프로덕션 빌드에 포함하지 말 것.
    /// 사용법: 빈 GameObject에 컴포넌트 추가 후 Play Mode 진입.
    /// </summary>
    public class V03_DefaultValueValidation : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(V03_DefaultValueValidation)}]";
        [SerializeField] private CharacterStatsSO _testCharacterStats;
        private void Start()
        {
            RunTest().Forget();
            RunValidationAsync().Forget();
        }

        private async UniTaskVoid RunTest()
        {
            var runRepo = GlobalBootstrapper.Instance.GameContext.CharacterRunRepo;
            var accountRepo = GlobalBootstrapper.Instance.GameContext.CharacterAccountRepo;

            // V-03: 기본값 확인
            Debug.Log($"[Test] V-03 - Hp: {runRepo.RunData.Hp}, Day: {runRepo.RunData.Day}, Gold: {runRepo.RunData.Gold}");
            Debug.Log($"[Test] V-03 - Gems: {accountRepo.AccountData.Gems}, UnlockedNodes: {accountRepo.AccountData.UnlockedEvolutionNodeIds.Count}");

            // V-04: InitializeNewRun으로 _isDirty = true 만든 후 저장
            runRepo.InitializeNewRun("test_node_id", _testCharacterStats);
            await accountRepo.SaveDataAsync();
            Debug.Log($"[Test] V-04 - SavePath: {Application.persistentDataPath}");
        }

        private async UniTask RunValidationAsync()
        {
            Debug.Log(Application.persistentDataPath);
            Debug.Log($"{_logClass} === V-03 검증 시작: 저장 파일 없을 때 기본값 초기화 ===");

            var runSavePath     = Application.persistentDataPath + "/run_save.json";
            var accountSavePath = Application.persistentDataPath + "/account_save.json";
            var runBackupPath     = runSavePath     + ".v03.bak";
            var accountBackupPath = accountSavePath + ".v03.bak";

            // ── 기존 저장 파일 백업 (파괴 방지) ──────────────────────────
            bool runFileExisted     = File.Exists(runSavePath);
            bool accountFileExisted = File.Exists(accountSavePath);

            if (runFileExisted)     File.Move(runSavePath,     runBackupPath);
            if (accountFileExisted) File.Move(accountSavePath, accountBackupPath);

            try
            {
                // ── 저장 파일 없는 상태에서 Repository 생성 및 로드 ──────
                var runRepo     = new CharacterRunRepository();
                var accountRepo = new CharacterAccountRepository();

                await UniTask.WhenAll(
                    runRepo.LoadDataAsync(),
                    accountRepo.LoadDataAsync()
                );

                // ── CharacterRunData 기본값 검증 ─────────────────────────
                var runData = runRepo.RunData;

                bool runNotNull  = runData != null;
                bool runHp       = runNotNull && runData.Hp         == 0;
                bool runStr      = runNotNull && runData.Strength   == 0;
                bool runTough    = runNotNull && runData.Toughness  == 0;
                bool runSpeed    = runNotNull && runData.Speed      == 0;
                bool runEvoId    = runNotNull && runData.EvolutionNodeId == null;
                bool runDay      = runNotNull && runData.Day        == 0;
                bool runGold     = runNotNull && runData.Gold       == 0;
                bool runPass     = runNotNull && runHp && runStr && runTough
                                             && runSpeed && runEvoId && runDay && runGold;

                // ── CharacterAccountData 기본값 검증 ─────────────────────
                var accountData = accountRepo.AccountData;

                bool accNotNull    = accountData != null;
                bool accUnlocked   = accNotNull && accountData.UnlockedEvolutionNodeIds != null
                                               && accountData.UnlockedEvolutionNodeIds.Count == 0;
                bool accCodex      = accNotNull && accountData.CompletedCodexIds != null
                                               && accountData.CompletedCodexIds.Count == 0;
                bool accGems       = accNotNull && accountData.Gems == 0;
                bool accountPass   = accNotNull && accUnlocked && accCodex && accGems;

                // ── 결과 출력 ─────────────────────────────────────────────
                Debug.Log($"{_logClass} --- CharacterRunData ---");
                LogCheck("RunData != null",         runNotNull);
                LogCheck("Hp == 0",                 runHp);
                LogCheck("Strength == 0",           runStr);
                LogCheck("Toughness == 0",          runTough);
                LogCheck("Speed == 0",              runSpeed);
                LogCheck("EvolutionNodeId == null", runEvoId);
                LogCheck("Day == 0",                runDay);
                LogCheck("Gold == 0",               runGold);

                Debug.Log($"{_logClass} --- CharacterAccountData ---");
                LogCheck("AccountData != null",                  accNotNull);
                LogCheck("UnlockedEvolutionNodeIds != null",     accUnlocked);
                LogCheck("UnlockedEvolutionNodeIds.Count == 0",  accUnlocked);
                LogCheck("CompletedCodexIds != null",            accCodex);
                LogCheck("CompletedCodexIds.Count == 0",         accCodex);
                LogCheck("Gems == 0",                            accGems);

                // ── 최종 판정 ─────────────────────────────────────────────
                if (runPass && accountPass)
                    Debug.Log($"{_logClass} [PASS] V-03: 저장 파일 없을 때 기본값으로 올바르게 초기화됨.");
                else
                    Debug.LogError($"{_logClass} [FAIL] V-03: 기본값 초기화 실패. 위 로그를 확인하세요.");
            }
            finally
            {
                // ── 백업 파일 복구 ────────────────────────────────────────
                if (runFileExisted)     File.Move(runBackupPath,     runSavePath);
                if (accountFileExisted) File.Move(accountBackupPath, accountSavePath);

                Debug.Log($"{_logClass} === V-03 검증 종료. 기존 저장 파일 복구 완료. ===");
            }
        }

        private void LogCheck(string label, bool pass)
        {
            if (pass)
                Debug.Log($"{_logClass}   [PASS] {label}");
            else
                Debug.LogError($"{_logClass}   [FAIL] {label}");
        }
    }
}
