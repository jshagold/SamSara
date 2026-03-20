using System;
using Samsara.Features.Stage.Data;
using UnityEngine;

namespace Samsara.Dev.Stage
{
    /// <summary>
    /// [V-08] GetStageById() / GetNodeById()에 알 수 없는 ID를 전달 시
    /// InvalidOperationException이 발생하는지 검증.
    /// Dev 전용. 프로덕션 빌드에 포함하지 말 것.
    /// 사용법: 빈 GameObject에 컴포넌트 추가 후 Play Mode 진입.
    ///
    /// NOTE: Positive Control은 Resources/MasterData에 실제 StageSO 자산이 있을 때만 실행됨.
    /// </summary>
    public class V08_GetStageByIdExceptionValidation : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(V08_GetStageByIdExceptionValidation)}]";

        private void Start()
        {
            RunValidation();
        }

        private void RunValidation()
        {
            Debug.Log($"{_logClass} === V-08 검증 시작: 알 수 없는 ID → InvalidOperationException 발생 ===");

            var repo = new StageMasterDataRepository();
            repo.Initialize();

            // ════════════════════════════════════════════════════════════
            // Case 1 — GetStageById(): 알 수 없는 ID → InvalidOperationException
            // ════════════════════════════════════════════════════════════
            Debug.Log($"{_logClass} --- [Case 1] GetStageById(\"__nonexistent_stage__\") ---");
            bool case1Pass = false;
            try
            {
                repo.GetStageById("__nonexistent_stage__");
                Debug.LogError($"{_logClass}   [FAIL] 예외가 발생하지 않음 — InvalidOperationException 필요.");
            }
            catch (InvalidOperationException ex)
            {
                case1Pass = true;
                Debug.Log($"{_logClass}   [PASS] InvalidOperationException 발생 (메시지: {ex.Message})");
            }
            catch (Exception ex)
            {
                Debug.LogError($"{_logClass}   [FAIL] 잘못된 예외 타입: {ex.GetType().Name} — {ex.Message}");
            }

            // ════════════════════════════════════════════════════════════
            // Case 2 — GetNodeById(): 알 수 없는 ID → InvalidOperationException
            // ════════════════════════════════════════════════════════════
            Debug.Log($"{_logClass} --- [Case 2] GetNodeById(\"__nonexistent_node__\") ---");
            bool case2Pass = false;
            try
            {
                repo.GetNodeById("__nonexistent_node__");
                Debug.LogError($"{_logClass}   [FAIL] 예외가 발생하지 않음 — InvalidOperationException 필요.");
            }
            catch (InvalidOperationException ex)
            {
                case2Pass = true;
                Debug.Log($"{_logClass}   [PASS] InvalidOperationException 발생 (메시지: {ex.Message})");
            }
            catch (Exception ex)
            {
                Debug.LogError($"{_logClass}   [FAIL] 잘못된 예외 타입: {ex.GetType().Name} — {ex.Message}");
            }

            // ════════════════════════════════════════════════════════════
            // Case 3 — Positive Control: 실제 존재하는 ID → 예외 없이 반환
            // (Resources/MasterData에 StageSO 자산이 있을 때만 실행)
            // ════════════════════════════════════════════════════════════
            Debug.Log($"{_logClass} --- [Case 3] Positive Control ---");
            var allStages = repo.GetAllStages();
            bool case3Pass = true;

            if (allStages.Count > 0)
            {
                var first = allStages[0];
                try
                {
                    var result = repo.GetStageById(first.StageId);
                    case3Pass = result == first;
                    LogCheck($"GetStageById(\"{first.StageId}\") → 올바른 StageSO 반환", case3Pass);
                }
                catch (Exception ex)
                {
                    case3Pass = false;
                    Debug.LogError($"{_logClass}   [FAIL] 알려진 ID에서 예외 발생: {ex.GetType().Name} — {ex.Message}");
                }
            }
            else
            {
                Debug.Log($"{_logClass}   [SKIP] Resources/MasterData에 StageSO 없음. 자산 생성 후 재검증 요망.");
            }

            // ── 최종 판정 ────────────────────────────────────────────
            Debug.Log($"{_logClass} -------------------------------------------");
            bool allPass = case1Pass && case2Pass && case3Pass;
            if (allPass)
                Debug.Log($"{_logClass} [PASS] V-08: 알 수 없는 ID에 대해 InvalidOperationException 정상 발생.");
            else
                Debug.LogError($"{_logClass} [FAIL] V-08: 예외 동작 불일치. 위 로그를 확인하세요.");

            Debug.Log($"{_logClass} === V-08 검증 종료. ===");
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
