using Samsara.Features.Stage.Data;
using Samsara.Features.Stage.MasterData;
using UnityEngine;

namespace Samsara.Dev.Stage
{
    /// <summary>
    /// [V-09] Initialize() 후 Resources/MasterData의 모든 StageSO / StageNodeSO가
    /// 캐시에 올바르게 로드되는지 검증.
    /// Dev 전용. 프로덕션 빌드에 포함하지 말 것.
    /// 사용법: 빈 GameObject에 컴포넌트 추가 후 Play Mode 진입.
    ///
    /// NOTE: Resources/MasterData에 StageSO 자산이 없으면 Count=0으로 PASS 처리.
    ///       자산 생성 후 재검증을 권장.
    /// </summary>
    public class V09_MasterDataCacheValidation : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(V09_MasterDataCacheValidation)}]";

        private void Start()
        {
            RunValidation();
        }

        private void RunValidation()
        {
            Debug.Log($"{_logClass} === V-09 검증 시작: Initialize() 후 MasterData 캐시 로드 확인 ===");

            var repo = new StageMasterDataRepository();
            repo.Initialize();

            // ── Resources 직접 조회 (ground truth) ──────────────────────
            var rawStages = Resources.LoadAll<StageSO>("MasterData");
            var rawNodes  = Resources.LoadAll<StageNodeSO>("MasterData");

            Debug.Log($"{_logClass} --- [Resources 직접 조회 결과] ---");
            Debug.Log($"{_logClass}   Resources.LoadAll<StageSO>     : {rawStages.Length}개");
            Debug.Log($"{_logClass}   Resources.LoadAll<StageNodeSO> : {rawNodes.Length}개");

            // ── GetAllStages() 수 일치 확인 ─────────────────────────────
            var cachedStages = repo.GetAllStages();
            bool stageCountMatch = cachedStages.Count == rawStages.Length;

            Debug.Log($"{_logClass} --- [캐시 수 검증] ---");
            LogCheck($"GetAllStages().Count({cachedStages.Count}) == Resources StageSO({rawStages.Length})",
                stageCountMatch, cachedStages.Count.ToString());

            // ── 각 StageSO가 GetStageById()로 조회 가능한지 확인 ─────────
            Debug.Log($"{_logClass} --- [개별 GetStageById() 접근성 검증] ---");
            bool allStagesAccessible = true;

            foreach (var stage in rawStages)
            {
                bool found = false;
                try
                {
                    var cached = repo.GetStageById(stage.StageId);
                    found = cached != null && cached == stage;
                }
                catch { }

                allStagesAccessible &= found;
                LogCheck($"GetStageById(\"{stage.StageId}\") → 캐시 존재", found);
            }

            // ── 각 StageNodeSO가 GetNodeById()로 조회 가능한지 확인 ──────
            Debug.Log($"{_logClass} --- [개별 GetNodeById() 접근성 검증] ---");
            bool allNodesAccessible = true;

            foreach (var node in rawNodes)
            {
                bool found = false;
                try
                {
                    var cached = repo.GetNodeById(node.NodeId);
                    found = cached != null && cached == node;
                }
                catch { }

                allNodesAccessible &= found;
                LogCheck($"GetNodeById(\"{node.NodeId}\") → 캐시 존재", found);
            }

            if (rawStages.Length == 0 && rawNodes.Length == 0)
                Debug.Log($"{_logClass}   [INFO] Resources/MasterData에 Stage 관련 자산 없음. 자산 생성 후 재검증 요망.");

            // ── 최종 판정 ────────────────────────────────────────────────
            Debug.Log($"{_logClass} -------------------------------------------");
            bool allPass = stageCountMatch && allStagesAccessible && allNodesAccessible;
            if (allPass)
                Debug.Log($"{_logClass} [PASS] V-09: Initialize() 후 모든 StageSO / StageNodeSO가 캐시에 로드됨.");
            else
                Debug.LogError($"{_logClass} [FAIL] V-09: 캐시 불일치. 위 로그를 확인하세요.");

            Debug.Log($"{_logClass} === V-09 검증 종료. ===");
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
