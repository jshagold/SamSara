using System;
using System.IO;
using Cysharp.Threading.Tasks;
using Samsara.Features.Stage.Data;
using UnityEngine;

namespace Samsara.Dev.Stage
{
    /// <summary>
    /// [V-04] InitializeRun() 후 stage_run_save.json이 persistentDataPath에 생성되는지 검증.
    /// Dev 전용. 프로덕션 빌드에 포함하지 말 것.
    /// 사용법: 빈 GameObject에 컴포넌트 추가 후 Play Mode 진입.
    /// </summary>
    public class V04_SaveFileCreationValidation : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(V04_SaveFileCreationValidation)}]";

        private void Start()
        {
            RunValidationAsync().Forget();
        }

        private async UniTask RunValidationAsync()
        {
            Debug.Log($"{_logClass} === V-04 검증 시작: SaveAsync() 후 stage_run_save.json 생성 확인 ===");

            var savePath   = Application.persistentDataPath + "/stage_run_save.json";
            var backupPath = savePath + ".v04.bak";

            bool fileExisted = File.Exists(savePath);
            if (fileExisted) File.Move(savePath, backupPath);

            var runConfig = Resources.Load<RunConfigSO>("MasterData/DefaultRunConfig");
            if (runConfig == null)
                throw new InvalidOperationException(
                    $"{_logClass} DefaultRunConfig.asset이 Resources/MasterData/에 없습니다.");

            try
            {
                // ── SaveAsync() 전: 파일 미존재 확인 ─────────────────────────
                bool fileAbsentBefore = !File.Exists(savePath);
                LogCheck("SaveAsync() 전: stage_run_save.json 없음", fileAbsentBefore);

                // ── InitializeNewRun → SaveAsync() 경유 파일 생성 ─
                var repo = new StageRepository();
                repo.InitializeNewRun(runConfig);
                await repo.SaveAsync();

                bool fileCreated = File.Exists(savePath);
                LogCheck($"SaveAsync() 후: stage_run_save.json 생성됨", fileCreated,
                    fileCreated ? savePath : "파일 없음");

                // ── 저장 경로가 persistentDataPath 하위인지 확인 ─────────────
                bool correctDir = savePath.StartsWith(Application.persistentDataPath);
                LogCheck("저장 경로가 persistentDataPath 하위임", correctDir, savePath);

                // ── 파일명이 stage_run_save.json인지 확인 ─────────────────────
                bool correctName = savePath.EndsWith("/stage_run_save.json")
                                || savePath.EndsWith("\\stage_run_save.json");
                LogCheck("파일명 == stage_run_save.json", correctName, savePath);

                Debug.Log($"{_logClass} -------------------------------------------");
                bool allPass = fileAbsentBefore && fileCreated && correctDir && correctName;
                if (allPass)
                    Debug.Log($"{_logClass} [PASS] V-04: stage_run_save.json 정상 생성됨 (경로={savePath})");
                else
                    Debug.LogError($"{_logClass} [FAIL] V-04: 파일 생성 실패. 위 로그를 확인하세요.");
            }
            finally
            {
                if (File.Exists(savePath)) File.Delete(savePath);
                if (fileExisted) File.Move(backupPath, savePath);
                Debug.Log($"{_logClass} === V-04 검증 종료. 기존 저장 파일 복구 완료. ===");
            }
        }

        private void LogCheck(string label, bool pass, string actual = null)
        {
            if (pass)
                Debug.Log($"{_logClass}   [PASS] {label}");
            else
                Debug.LogError($"{_logClass}   [FAIL] {label}{(actual != null ? $" (실제={actual})" : "")}");
        }
    }
}
