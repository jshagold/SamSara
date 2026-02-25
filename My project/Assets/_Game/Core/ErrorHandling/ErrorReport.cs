using Newtonsoft.Json;

namespace Core.ErrorHandling
{
    /// <summary>
    /// DTO: Report payload for transmission/buffer (FR-08). JSON-serializable.
    /// </summary>
    public class ErrorReport
    {
        [JsonProperty("severity")]
        public ErrorSeverity Severity { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("stackTrace")]
        public string StackTrace { get; set; }

        [JsonProperty("snapshotId")]
        public string SnapshotId { get; set; }

        [JsonProperty("timestampUtc")]
        public long TimestampUtc { get; set; }

        [JsonProperty("contextScene")]
        public string ContextScene { get; set; }

        [JsonProperty("contextFrame")]
        public int ContextFrame { get; set; }
    }
}
