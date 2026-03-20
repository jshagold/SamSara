using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using Samsara.Features.Stage.Data;
using UnityEngine;

namespace Samsara.Dev.Stage
{
    /// <summary>
    /// [V-07] TransitionToStage() 호출 시 GeneratedNodeIds와 CompletedNodeIndices가
    /// 클리어되고 CurrentStageId / CurrentNodeIndex가 올바르게 설정되는지 검증.
    /// Dev 전용. 프로덕션 빌드에 포함하지 말 것.
    /// 사용법: 빈 GameObject에 컴포넌트 추가 후 Play Mode 진입.
    ///
    /// NOTE: 인메모리 상태 검증이므로 AsyncSave 완료 대기 없이 즉시 확인 가능.
    ///       finally에서 잔류 파일 정리 시에만 Delay 사용.
    /// </summary>
    public class V07_TransitionToStageValidation : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(V07_TransitionToStageValidation)}]";

        private const string Stage01 = "v07_stage_01";
        private const string Stage02 = "v07_stage_02";

        private void Start()
        {
            RunValidationAsync().Forget();
        }

        private async UniTask RunValidationAsync()
        {
            Debug.Log($"{_logClass} === V-07 검증 시작: TransitionToStage() 상태 초기화 검증 ===");

            var savePath   = Application.persistentDataPath + "/stage_run_save.json";
            var backupPath = savePath + ".v07.bak";

            bool fileExisted = File.Exists(savePath);
            if (fileExisted) File.Move(savePath, backupPath);

            try
            {
                var repo = new StageRepository();

                // ── Phase 1: 초기 상태 구성 ──────────────────────────────
                repo.InitializeRun(Stage01);
                repo.SetGeneratedNodes(new List<string> { "node_a", "node_b", "node_c" });
                repo.CompleteNode(0);   // CompletedNodeIndices=[0], CurrentNodeIndex=1
                repo.CompleteNode(1);   // CompletedNodeIndices=[0,1], CurrentNodeIndex=2

                var d = repo.RunData;

                Debug.Log($"{_logClass} --- [전이 전 상태 확인] ---");
                bool preStageId   = d.CurrentStageId            == Stage01;
                bool preGenCount  = d.GeneratedNodeIds.Count    == 3;
                bool preCompCount = d.CompletedNodeIndices.Count == 2;
                bool preIndex     = d.CurrentNodeIndex           == 2;

                LogCheck($"CurrentStageId == \"{Stage01}\"",    preStageId,   $"\"{d.CurrentStageId}\"");
                LogCheck("GeneratedNodeIds.Count == 3",          preGenCount,  d.GeneratedNodeIds.Count.ToString());
                LogCheck("CompletedNodeIndices.Count == 2",      preCompCount, d.CompletedNodeIndices.Count.ToString());
                LogCheck("CurrentNodeIndex == 2",                preIndex,     d.CurrentNodeIndex.ToString());

                // ── Phase 2: TransitionToStage 호출 ─────────────────────
                repo.TransitionToStage(Stage02);

                // ── Phase 3: 전이 후 인메모리 상태 즉시 검증 ─────────────
                Debug.Log($"{_logClass} --- [전이 후 상태 검증] ---");
                bool postStageId   = d.CurrentStageId            == Stage02;
                bool postGenEmpty  = d.GeneratedNodeIds.Count    == 0;
                bool postCompEmpty = d.CompletedNodeIndices.Count == 0;
                bool postIndex     = d.CurrentNodeIndex           == 0;

                LogCheck($"CurrentStageId == \"{Stage02}\" (새 스테이지로 변경)",    postStageId,   $"\"{d.CurrentStageId}\"");
                LogCheck("GeneratedNodeIds.Count == 0 (클리어됨)",                  postGenEmpty,  d.GeneratedNodeIds.Count.ToString());
                LogCheck("CompletedNodeIndices.Count == 0 (클리어됨)",              postCompEmpty, d.CompletedNodeIndices.Count.ToString());
                LogCheck("CurrentNodeIndex == 0 (리셋됨)",                          postIndex,     d.CurrentNodeIndex.ToString());

                bool allPass = preStageId && preGenCount && preCompCount && preIndex
                            && postStageId && postGenEmpty && postCompEmpty && postIndex;

                Debug.Log($"{_logClass} -------------------------------------------");
                if (allPass)
                    Debug.Log($"{_logClass} [PASS] V-07: TransitionToStage()가 GeneratedNodeIds와 CompletedNodeIndices를 정상 초기화함.");
                else
                    Debug.LogError($"{_logClass} [FAIL] V-07: 전이 후 상태 불일치. 위 로그를 확인하세요.");
            }
            finally
            {
                // 내부 SaveAsync().Forget() 완료 후 잔류 파일 정리
                await UniTask.Delay(500);
                if (File.Exists(savePath)) File.Delete(savePath);
                if (fileExisted) File.Move(backupPath, savePath);
                Debug.Log($"{_logClass} === V-07 검증 종료. ===");
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
