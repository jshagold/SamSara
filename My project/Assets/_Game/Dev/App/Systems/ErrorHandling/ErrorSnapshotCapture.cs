using System;
using Cysharp.Threading.Tasks;
using Core.ErrorHandling;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace App.Systems.ErrorHandling
{
    /// <summary>
    /// Builds StateSnapshot for error diagnostics (FR-04). Uses Scene/Time/SystemInfo as read-only diagnostic APIs only.
    /// </summary>
    public class ErrorSnapshotCapture
    {
        private readonly GameContext _gameContext;
        private readonly LogRingBuffer _ringBuffer;

        public ErrorSnapshotCapture(GameContext gameContext, LogRingBuffer ringBuffer)
        {
            _gameContext = gameContext ?? throw new ArgumentNullException(nameof(gameContext));
            _ringBuffer = ringBuffer ?? throw new ArgumentNullException(nameof(ringBuffer));
        }

        public async UniTask<StateSnapshot> CaptureAsync()
        {
            string sceneName = null;
            int frameCount = 0;
            string deviceModel = null;
            string operatingSystem = null;
            string dataSummaryJson = null;
            LogRingBufferEntry[] ringPayload = null;

            await UniTask.SwitchToMainThread();
            sceneName = SceneManager.GetActiveScene().name;
            frameCount = Time.frameCount;
            deviceModel = SystemInfo.deviceModel;
            operatingSystem = SystemInfo.operatingSystem;
            dataSummaryJson = _gameContext.GetDataSummaryJson();
            ringPayload = _ringBuffer.GetSnapshot();

            return new StateSnapshot
            {
                SnapshotId = Guid.NewGuid().ToString("N"),
                SceneName = sceneName,
                FrameCount = frameCount,
                DeviceModel = deviceModel,
                OperatingSystem = operatingSystem,
                MemoryUsage = null,
                DataSummaryJson = dataSummaryJson,
                RingBufferPayload = ringPayload
            };
        }
    }
}
