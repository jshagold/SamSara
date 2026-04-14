using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Samsara.Features.Stage.Data;
using Samsara.Features.Stage.Domain;
using UnityEngine;

namespace Samsara.Dev.Stage
{
    /// <summary>
    /// [DEV ONLY] StageRunData를 Inspector에서 직접 수정하는 임시 디버그 도구.
    /// Phase 6 정식 게임 시작 흐름 구현 전까지 테스트용으로 사용.
    /// 사용법:
    ///   1. Bootstrap 씬의 빈 GameObject에 컴포넌트 추가
    ///   2. Play Mode 진입
    ///   3. Inspector에서 원하는 값 수정 후 컨텍스트 메뉴 → Apply / Load 호출
    /// </summary>
    public class DebugStageRunDataEditor : MonoBehaviour
    {
        private readonly string _logClass = $"[{nameof(DebugStageRunDataEditor)}]";

        [Header("적용할 값")]
        [SerializeField] private string _currentStageId = "";
        [SerializeField] private int _currentNodeIndex;
        [SerializeField] private List<string> _generatedNodeIds = new();
        [SerializeField] private List<int> _completedNodeIndices = new();

        [Header("옵션")]
        [SerializeField] private bool _loadOnStart = true;
        [SerializeField] private bool _applyOnStart = false;

        /// <summary>컴포넌트 추가 또는 Inspector Reset 시 RunConfigSO 기반으로 기본값을 설정한다.</summary>
        private void Reset()
        {
            var runConfig = Resources.Load<RunConfigSO>("MasterData/DefaultRunConfig");
            if (runConfig == null) return;

            _currentStageId = runConfig.StartStageId.ToString();
        }

        private void Start()
        {
            if (_loadOnStart) LoadFromRepo();
            if (_applyOnStart) ApplyToRepo();
        }

        /// <summary>현재 저장된 데이터를 Inspector 필드에 불러온다.</summary>
        [ContextMenu("Load from Repo")]
        private void LoadFromRepo()
        {
            var repo = GetRepo();
            if (repo == null) return;

            var d = repo.RunData;
            _currentStageId = d.CurrentStageId;
            _currentNodeIndex = d.CurrentNodeIndex;
            _generatedNodeIds = new List<string>(d.GeneratedNodeIds);
            _completedNodeIndices = new List<int>(d.CompletedNodeIndices);

            Debug.Log($"{_logClass} Repo 데이터 로드 완료. StageId={_currentStageId}");
        }

        /// <summary>Inspector 필드의 값을 Repo에 즉시 반영하고 저장한다.</summary>
        [ContextMenu("Apply to Repo")]
        private void ApplyToRepo()
        {
            var repo = GetRepo();
            if (repo == null) return;

            var runConfig = Resources.Load<RunConfigSO>("MasterData/DefaultRunConfig");
            if (runConfig == null)
                throw new InvalidOperationException(
                    $"{_logClass} DefaultRunConfig.asset이 Resources/MasterData/에 없습니다.");

            repo.InitializeNewRun(runConfig);

            var d = repo.RunData;
            d.CurrentStageId = _currentStageId;
            d.CurrentNodeIndex = _currentNodeIndex;
            d.GeneratedNodeIds = new List<string>(_generatedNodeIds);
            d.CompletedNodeIndices = new List<int>(_completedNodeIndices);

            repo.SaveAsync().Forget();

            Debug.Log($"{_logClass} Repo에 적용 및 저장 완료. StageId={_currentStageId}, NodeIndex={_currentNodeIndex}");
        }

        private IStageRepository GetRepo()
        {
            if (GlobalBootstrapper.Instance == null)
            {
                Debug.LogWarning($"{_logClass} GlobalBootstrapper.Instance가 null. Play Mode에서 실행하세요.");
                return null;
            }

            var repo = GlobalBootstrapper.Instance.GameContext.StageRepo;
            if (repo.RunData == null)
            {
                Debug.LogWarning($"{_logClass} RunData가 null. LoadAllDataAsync 완료 후 사용하세요.");
                return null;
            }

            return repo;
        }
    }
}
