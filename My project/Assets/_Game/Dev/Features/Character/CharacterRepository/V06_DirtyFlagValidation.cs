using System.IO;
using Cysharp.Threading.Tasks;
using Samsara.Features.Character.Data;
using UnityEngine;

namespace Samsara.Dev
{
    /// <summary>
    /// [V-06] Dirty Flag가 false일 때 SaveDataAsync()가 파일을 쓰지 않는지 검증.
    /// Dev 전용. 프로덕션 빌드에 포함하지 말 것.
    /// 사용법: 빈 GameObject에 컴포넌트 추가 후 Play Mode 진입.
    /// </summary>
    public class V06_DirtyFlagValidation : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(V06_DirtyFlagValidation)}]";

        private void Start()
        {
            RunValidationAsync().Forget();
        }

        private async UniTask RunValidationAsync()
        {
            Debug.Log($"{_logClass} === V-06 검증 시작: Dirty Flag false일 때 SaveDataAsync() 파일 쓰기 스킵 ===");

            var runSavePath     = Application.persistentDataPath + "/run_save.json";
            var accountSavePath = Application.persistentDataPath + "/account_save.json";
            var runBackupPath     = runSavePath     + ".v06.bak";
            var accountBackupPath = accountSavePath + ".v06.bak";

            bool runFileExisted     = File.Exists(runSavePath);
            bool accountFileExisted = File.Exists(accountSavePath);

            if (runFileExisted)     File.Move(runSavePath,     runBackupPath);
            if (accountFileExisted) File.Move(accountSavePath, accountBackupPath);

            try
            {
                // ════════════════════════════════════════════════════════════
                // Case 1 — CharacterRunRepository: Dirty=false → 파일 미생성
                // ════════════════════════════════════════════════════════════
                Debug.Log($"{_logClass} --- [Case 1] CharacterRunRepository: Dirty=false ---");

                var runRepo = new CharacterRunRepository();
                await runRepo.LoadDataAsync();          // _isDirty 는 여기서도 설정되지 않음

                long runTimestampBefore = GetFileTimestamp(runSavePath);
                await runRepo.SaveDataAsync();          // Dirty=false → 스킵이어야 함
                long runTimestampAfter  = GetFileTimestamp(runSavePath);

                bool runSkipped = !File.Exists(runSavePath) || runTimestampAfter == runTimestampBefore;
                LogCheck("Dirty=false: run_save.json 미생성/미변경", runSkipped);

                // ════════════════════════════════════════════════════════════
                // Case 2 — CharacterAccountRepository: Dirty=false → 파일 미생성
                // ════════════════════════════════════════════════════════════
                Debug.Log($"{_logClass} --- [Case 2] CharacterAccountRepository: Dirty=false ---");

                var accountRepo = new CharacterAccountRepository();
                await accountRepo.LoadDataAsync();      // _isDirty 는 여기서도 설정되지 않음

                long accTimestampBefore = GetFileTimestamp(accountSavePath);
                await accountRepo.SaveDataAsync();      // Dirty=false → 스킵이어야 함
                long accTimestampAfter  = GetFileTimestamp(accountSavePath);

                bool accSkipped = !File.Exists(accountSavePath) || accTimestampAfter == accTimestampBefore;
                LogCheck("Dirty=false: account_save.json 미생성/미변경", accSkipped);

                // ════════════════════════════════════════════════════════════
                // Case 3 — Positive Control: Dirty=true → 파일 생성됨 (테스트 신뢰성 보장)
                // Case 1·2가 "SaveDataAsync 자체가 항상 아무것도 안 함"이 아님을 확인.
                // ════════════════════════════════════════════════════════════
                Debug.Log($"{_logClass} --- [Case 3] Positive Control: Dirty=true → 파일 생성 확인 ---");

                // account_save.json 이 없는 상태에서 시작
                var accountRepoPositive = new CharacterAccountRepository();
                await accountRepoPositive.LoadDataAsync();      // default AccountData, _isDirty=false

                accountRepoPositive.UnlockEvolutionNode("v06_control_node");  // _isDirty=true 로 변경

                await accountRepoPositive.SaveDataAsync();      // Dirty=true → 파일 써야 함

                bool positiveFileCreated = File.Exists(accountSavePath);
                LogCheck("Dirty=true: account_save.json 생성됨 (Positive Control)", positiveFileCreated);

                // 생성된 positive control 파일 제거 (이후 복구에서 기존 파일만 남기기 위해)
                if (positiveFileCreated) File.Delete(accountSavePath);

                // ── 최종 판정 ────────────────────────────────────────────
                Debug.Log($"{_logClass} -------------------------------------------");
                bool allPass = runSkipped && accSkipped && positiveFileCreated;
                if (allPass)
                    Debug.Log($"{_logClass} [PASS] V-06: Dirty Flag false → SaveDataAsync() 파일 쓰기 정상 스킵.");
                else
                    Debug.LogError($"{_logClass} [FAIL] V-06: 예상과 다른 동작 감지. 위 로그를 확인하세요.");
            }
            finally
            {
                // ── 테스트 잔류 파일 제거 & 기존 파일 복구 ──────────────
                if (File.Exists(runSavePath))     File.Delete(runSavePath);
                if (File.Exists(accountSavePath)) File.Delete(accountSavePath);

                if (runFileExisted)     File.Move(runBackupPath,     runSavePath);
                if (accountFileExisted) File.Move(accountBackupPath, accountSavePath);

                Debug.Log($"{_logClass} === V-06 검증 종료. 기존 저장 파일 복구 완료. ===");
            }
        }

        /// <summary>
        /// 파일이 존재하면 LastWriteTime ticks 반환. 존재하지 않으면 -1.
        /// SaveDataAsync() 호출 전후 timestamp를 비교해 파일 쓰기 여부를 판별.
        /// </summary>
        private static long GetFileTimestamp(string path)
        {
            return File.Exists(path) ? new FileInfo(path).LastWriteTimeUtc.Ticks : -1L;
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
