using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using Samsara.Features.Stage.Data;
using UnityEngine;

namespace Samsara.Dev.Stage
{
    /// <summary>
    /// [V-05] SaveAsync() 후 새 인스턴스에서 LoadAsync() 시 값이 올바르게 복원되는지 검증.
    /// Dev 전용. 프로덕션 빌드에 포함하지 말 것.
    /// 사용법: 빈 GameObject에 컴포넌트 추가 후 Play Mode 진입.
    ///
    /// NOTE: mutation 메서드들이 SaveAsync().Forget()을 내부 호출하므로
    ///       순차 Delay를 삽입해 각 저장이 완료된 이후 다음 mutation을 실행한다.
    /// </summary>
    public class V05_SaveRestoreValidation : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(V05_SaveRestoreValidation)}]";

        private const string TestStageId = "v05_stage_01";
        private const string TestNodeA   = "v05_node_a";
        private const string TestNodeB   = "v05_node_b";
        private const string TestNodeC   = "v05_node_c";

        private void Start()
        {
            RunValidationAsync().Forget();
        }

        private async UniTask RunValidationAsync()
        {
            Debug.Log($"{_logClass} === V-05 검증 시작: SaveAsync() 후 값 복원 ===");

            var savePath   = Application.persistentDataPath + "/stage_run_save.json";
            var backupPath = savePath + ".v05.bak";

            bool fileExisted = File.Exists(savePath);
            if (fileExisted) File.Move(savePath, backupPath);

            try
            {
                // ════════════════════════════════════════════════════════════
                // Phase 1 — 테스트 값 기록 (저장)
                // mutation마다 내부 SaveAsync().Forget()이 발생하므로
                // 각 호출 후 Delay를 삽입하여 쓰기 경쟁을 방지한다.
                // ════════════════════════════════════════════════════════════
                var writerRepo = new StageRepository();

                writerRepo.InitializeRun(TestStageId);
                await UniTask.Delay(300);   // Forget 완료 대기

                writerRepo.SetGeneratedNodes(new List<string> { TestNodeA, TestNodeB, TestNodeC });
                await UniTask.Delay(300);

                writerRepo.CompleteNode(0); // CompletedNodeIndices=[0], CurrentNodeIndex=1
                await UniTask.Delay(300);

                Debug.Log($"{_logClass} Phase 1 완료: 테스트 값 저장됨.");

                // ════════════════════════════════════════════════════════════
                // Phase 2 — 새 인스턴스로 로드 (다음 실행 시뮬레이션)
                // ════════════════════════════════════════════════════════════
                var readerRepo = new StageRepository();
                await readerRepo.LoadAsync();

                Debug.Log($"{_logClass} Phase 2 완료: 새 인스턴스에서 로드됨.");

                // ════════════════════════════════════════════════════════════
                // Phase 3 — 복원 값 검증
                // ════════════════════════════════════════════════════════════
                var d = readerRepo.RunData;

                bool notNull      = d != null;
                bool stageId      = notNull && d.CurrentStageId == TestStageId;
                bool nodeIndex    = notNull && d.CurrentNodeIndex == 1;
                bool genCount     = notNull && d.GeneratedNodeIds != null && d.GeneratedNodeIds.Count == 3;
                bool genNodeA     = genCount && d.GeneratedNodeIds[0] == TestNodeA;
                bool genNodeB     = genCount && d.GeneratedNodeIds[1] == TestNodeB;
                bool genNodeC     = genCount && d.GeneratedNodeIds[2] == TestNodeC;
                bool compCount    = notNull && d.CompletedNodeIndices != null && d.CompletedNodeIndices.Count == 1;
                bool compNode0    = compCount && d.CompletedNodeIndices[0] == 0;

                Debug.Log($"{_logClass} --- [StageRunData 복원 검증] ---");
                LogCheck($"CurrentStageId == \"{TestStageId}\"",               stageId,   $"\"{d?.CurrentStageId}\"");
                LogCheck("CurrentNodeIndex == 1 (CompleteNode(0) 이후)",       nodeIndex,  d?.CurrentNodeIndex.ToString());
                LogCheck("GeneratedNodeIds.Count == 3",                        genCount,   d?.GeneratedNodeIds?.Count.ToString());
                LogCheck($"GeneratedNodeIds[0] == \"{TestNodeA}\"",            genNodeA,   d?.GeneratedNodeIds?.Count > 0 ? $"\"{d.GeneratedNodeIds[0]}\"" : "N/A");
                LogCheck($"GeneratedNodeIds[1] == \"{TestNodeB}\"",            genNodeB,   d?.GeneratedNodeIds?.Count > 1 ? $"\"{d.GeneratedNodeIds[1]}\"" : "N/A");
                LogCheck($"GeneratedNodeIds[2] == \"{TestNodeC}\"",            genNodeC,   d?.GeneratedNodeIds?.Count > 2 ? $"\"{d.GeneratedNodeIds[2]}\"" : "N/A");
                LogCheck("CompletedNodeIndices.Count == 1",                    compCount,  d?.CompletedNodeIndices?.Count.ToString());
                LogCheck("CompletedNodeIndices[0] == 0",                       compNode0,  d?.CompletedNodeIndices?.Count > 0 ? d.CompletedNodeIndices[0].ToString() : "N/A");

                bool allPass = notNull && stageId && nodeIndex
                            && genCount && genNodeA && genNodeB && genNodeC
                            && compCount && compNode0;

                Debug.Log($"{_logClass} -------------------------------------------");
                if (allPass)
                    Debug.Log($"{_logClass} [PASS] V-05: SaveAsync() 후 모든 값이 정확히 복원됨.");
                else
                    Debug.LogError($"{_logClass} [FAIL] V-05: 복원 실패 항목 있음. 위 로그를 확인하세요.");
            }
            finally
            {
                if (File.Exists(savePath)) File.Delete(savePath);
                if (fileExisted) File.Move(backupPath, savePath);
                Debug.Log($"{_logClass} === V-05 검증 종료. 기존 저장 파일 복구 완료. ===");
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
