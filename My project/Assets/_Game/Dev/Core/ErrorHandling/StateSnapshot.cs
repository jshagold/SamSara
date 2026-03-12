using Newtonsoft.Json;

namespace Core.ErrorHandling
{
    /// <summary>
    /// DTO: Whole-state snapshot for diagnostics (FR-04).
    /// </summary>
    public class StateSnapshot
    {
        [JsonProperty("snapshotId")]
        public string SnapshotId { get; set; }

        [JsonProperty("sceneName")]
        public string SceneName { get; set; }

        [JsonProperty("frameCount")]
        public int FrameCount { get; set; }

        [JsonProperty("deviceModel")]
        public string DeviceModel { get; set; }

        [JsonProperty("operatingSystem")]
        public string OperatingSystem { get; set; }

        [JsonProperty("memoryUsage")]
        public long? MemoryUsage { get; set; }

        [JsonProperty("dataSummaryJson")]
        public string DataSummaryJson { get; set; }

        [JsonProperty("ringBufferPayload")]
        public LogRingBufferEntry[] RingBufferPayload { get; set; }
    }
}
