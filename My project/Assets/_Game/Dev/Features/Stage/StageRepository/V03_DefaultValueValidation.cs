using System.IO;
using Cysharp.Threading.Tasks;
using Samsara.Features.Stage.Data;
using UnityEngine;

namespace Samsara.Dev.Stage
{
    /// <summary>
    /// [V-03] LoadAsync() — 저장 파일 없을 때 StageRunData가 기본값으로 초기화되는지 검증.
    /// Dev 전용. 프로덕션 빌드에 포함하지 말 것.
    /// 사용법: 빈 GameObject에 컴포넌트 추가 후 Play Mode 진입.
    /// </summary>
    public class V03_DefaultValueValidation : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(V03_DefaultValueValidation)}]";

        private void Start()
        {
            RunValidationAsync().Forget();
        }

        private async UniTask RunValidationAsync()
        {
            Debug.Log($"{_logClass} === V-03 검증 시작: 저장 파일 없을 때 기본값 초기화 ===");

            var savePath   = Application.persistentDataPath + "/stage_run_save.json";
            var backupPath = savePath + ".v03.bak";

            bool fileExisted = File.Exists(savePath);
            if (fileExisted) File.Move(savePath, backupPath);

            try
            {
                var repo = new StageRepository();
                await repo.LoadAsync();

                var d = repo.RunData;

                bool notNull       = d != null;
                bool stageIdNull   = notNull && d.CurrentStageId == null;
                bool nodeIndexZero = notNull && d.CurrentNodeIndex == 0;
                bool genNotNull    = notNull && d.GeneratedNodeIds != null;
                bool genEmpty      = genNotNull && d.GeneratedNodeIds.Count == 0;
                bool compNotNull   = notNull && d.CompletedNodeIndices != null;
                bool compEmpty     = compNotNull && d.CompletedNodeIndices.Count == 0;

                Debug.Log($"{_logClass} --- [StageRunData 기본값 검증] ---");
                LogCheck("RunData != null",                            notNull);
                LogCheck("CurrentStageId == null",                    stageIdNull,   $"\"{d?.CurrentStageId}\"");
                LogCheck("CurrentNodeIndex == 0",                     nodeIndexZero,  d?.CurrentNodeIndex.ToString());
                LogCheck("GeneratedNodeIds != null",                  genNotNull,     d?.GeneratedNodeIds == null ? "null" : "not null");
                LogCheck("GeneratedNodeIds.Count == 0",               genEmpty,       d?.GeneratedNodeIds?.Count.ToString());
                LogCheck("CompletedNodeIndices != null",              compNotNull,    d?.CompletedNodeIndices == null ? "null" : "not null");
                LogCheck("CompletedNodeIndices.Count == 0",           compEmpty,      d?.CompletedNodeIndices?.Count.ToString());

                bool allPass = notNull && stageIdNull && nodeIndexZero
                            && genNotNull && genEmpty && compNotNull && compEmpty;

                Debug.Log($"{_logClass} -------------------------------------------");
                if (allPass)
                    Debug.Log($"{_logClass} [PASS] V-03: 저장 파일 없을 때 기본값으로 올바르게 초기화됨.");
                else
                    Debug.LogError($"{_logClass} [FAIL] V-03: 기본값 초기화 실패. 위 로그를 확인하세요.");
            }
            finally
            {
                if (File.Exists(savePath)) File.Delete(savePath);
                if (fileExisted) File.Move(backupPath, savePath);
                Debug.Log($"{_logClass} === V-03 검증 종료. 기존 저장 파일 복구 완료. ===");
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
