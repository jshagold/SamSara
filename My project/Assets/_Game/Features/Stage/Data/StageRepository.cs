using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Samsara.Features.Stage.Domain;
using UnityEngine;

namespace Samsara.Features.Stage.Data
{
    public class StageRepository : IStageRepository
    {
        private readonly string _logClass = $"[{nameof(StageRepository)}]";
        private readonly string _savePath = Application.persistentDataPath + "/stage_run_save.json";

        private StageRunData _runData;
        private bool _isDirty;

        public StageRunData RunData => _runData;

        // ──────────────────────────────────────────────
        // Mutation — Save-on-Action
        // ──────────────────────────────────────────────

        public void InitializeRun(string startStageId)
        {
            _runData = new StageRunData
            {
                CurrentStageId     = startStageId,
                CurrentNodeIndex   = 0,
                GeneratedNodeIds   = new List<string>(),
                CompletedNodeIndices = new List<int>()
            };
            _isDirty = true;
        }

        public void CompleteNode(int nodeIndex)
        {
            _runData.CompletedNodeIndices.Add(nodeIndex);
            _runData.CurrentNodeIndex++;
            _isDirty = true;
        }

        public void TransitionToStage(string stageId)
        {
            _runData.CurrentStageId = stageId;
            _runData.CurrentNodeIndex = 0;
            _runData.GeneratedNodeIds.Clear();
            _runData.CompletedNodeIndices.Clear();
            _isDirty = true;
        }

        public void SetGeneratedNodes(List<string> nodeIds)
        {
            _runData.GeneratedNodeIds = nodeIds;
            _isDirty = true;
        }

        public void SetPendingChainedEventId(int eventId)
        {
            _runData.PendingChainedEventId = eventId;
            _isDirty = true;
        }

        // ──────────────────────────────────────────────
        // Load
        // ──────────────────────────────────────────────

        public async UniTask LoadAsync()
        {
            await UniTask.RunOnThreadPool(() =>
            {
                if (File.Exists(_savePath))
                {
                    var json = File.ReadAllText(_savePath);
                    _runData = JsonConvert.DeserializeObject<StageRunData>(json);
                }
                else
                {
                    _runData = new StageRunData();
                }
            });

            _isDirty = false;
            Debug.Log($"{_logClass} LoadAsync 완료.");
        }

        // ──────────────────────────────────────────────
        // Save — Async (일반 게임플레이)
        // ──────────────────────────────────────────────

        public async UniTask SaveAsync()
        {
            if (!_isDirty) return;

            await UniTask.RunOnThreadPool(() =>
            {
                var json = JsonConvert.SerializeObject(_runData);
                File.WriteAllText(_savePath, json);
            });

            _isDirty = false;
            Debug.Log($"{_logClass} SaveAsync 완료.");
        }

        // ──────────────────────────────────────────────
        // Save — Sync (OnApplicationPause / OnApplicationQuit 전용)
        // ──────────────────────────────────────────────

        public void SaveSync()
        {
            if (!_isDirty) return;

            var json = JsonConvert.SerializeObject(_runData);
            File.WriteAllText(_savePath, json);
            _isDirty = false;

            Debug.Log($"{_logClass} SaveSync 완료.");
        }
    }
}
