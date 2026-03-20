using System.IO;
using Cysharp.Threading.Tasks;
using Samsara.Features.Stage.Data;
using UnityEngine;

namespace Samsara.Dev.Stage
{
    /// <summary>
    /// [V-06] Dirty Flag가 false일 때 SaveAsync() / SaveSync()가 파일을 쓰지 않는지 검증.
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
            Debug.Log($"{_logClass} === V-06 검증 시작: Dirty Flag false일 때 파일 쓰기 스킵 ===");

            var savePath   = Application.persistentDataPath + "/stage_run_save.json";
            var backupPath = savePath + ".v06.bak";

            bool fileExisted = File.Exists(savePath);
            if (fileExisted) File.Move(savePath, backupPath);

            try
            {
                // ════════════════════════════════════════════════════════════
                // Case 1 — SaveAsync(): Dirty=false → 파일 미생성
                // ════════════════════════════════════════════════════════════
                Debug.Log($"{_logClass} --- [Case 1] SaveAsync(): Dirty=false → 스킵 ---");

                var repo = new StageRepository();
                await repo.LoadAsync();   // 파일 없음 → 기본값 초기화, _isDirty=false

                long tsBefore = GetFileTimestamp(savePath);
                await repo.SaveAsync();   // _isDirty=false → 스킵이어야 함
                long tsAfter  = GetFileTimestamp(savePath);

                bool case1Pass = !File.Exists(savePath) || tsAfter == tsBefore;
                LogCheck("Dirty=false: SaveAsync() 후 stage_run_save.json 미생성/미변경", case1Pass);

                // ════════════════════════════════════════════════════════════
                // Case 2 — SaveSync(): Dirty=false → 파일 미생성
                // ════════════════════════════════════════════════════════════
                Debug.Log($"{_logClass} --- [Case 2] SaveSync(): Dirty=false → 스킵 ---");

                var repoSync = new StageRepository();
                await repoSync.LoadAsync();   // _isDirty=false

                long tsSyncBefore = GetFileTimestamp(savePath);
                repoSync.SaveSync();          // _isDirty=false → 스킵이어야 함
                long tsSyncAfter  = GetFileTimestamp(savePath);

                bool case2Pass = !File.Exists(savePath) || tsSyncAfter == tsSyncBefore;
                LogCheck("Dirty=false: SaveSync() 후 stage_run_save.json 미생성/미변경", case2Pass);

                // ════════════════════════════════════════════════════════════
                // Case 3 — Positive Control: Dirty=true → 파일 생성됨 (신뢰성 보장)
                // Case 1·2가 "Save 자체가 항상 아무것도 안 함"이 아님을 확인.
                // ════════════════════════════════════════════════════════════
                Debug.Log($"{_logClass} --- [Case 3] Positive Control: Dirty=true → 파일 생성 ---");

                var repoPositive = new StageRepository();
                await repoPositive.LoadAsync();          // _isDirty=false
                repoPositive.InitializeRun("v06_ctrl");  // _isDirty=true, Forget 내부 실행
                await UniTask.Delay(500);                // Forget 완료 대기

                bool case3Pass = File.Exists(savePath);
                LogCheck("Dirty=true: stage_run_save.json 생성됨 (Positive Control)", case3Pass);

                if (case3Pass) File.Delete(savePath);

                // ── 최종 판정 ────────────────────────────────────────────
                Debug.Log($"{_logClass} -------------------------------------------");
                bool allPass = case1Pass && case2Pass && case3Pass;
                if (allPass)
                    Debug.Log($"{_logClass} [PASS] V-06: Dirty Flag false → 파일 쓰기 정상 스킵.");
                else
                    Debug.LogError($"{_logClass} [FAIL] V-06: 예상과 다른 동작 감지. 위 로그를 확인하세요.");
            }
            finally
            {
                if (File.Exists(savePath)) File.Delete(savePath);
                if (fileExisted) File.Move(backupPath, savePath);
                Debug.Log($"{_logClass} === V-06 검증 종료. 기존 저장 파일 복구 완료. ===");
            }
        }

        /// <summary>파일이 존재하면 LastWriteTime ticks 반환. 없으면 -1.</summary>
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
