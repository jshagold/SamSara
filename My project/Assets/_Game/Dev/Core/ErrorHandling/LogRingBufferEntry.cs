using UnityEngine;

namespace Core.ErrorHandling
{
    /// <summary>
    /// Entry in the log ring buffer (context only, non-allocating). Uses UnityEngine.LogType per project spec (L3).
    /// </summary>
    public struct LogRingBufferEntry
    {
        public LogType LogType;
        public string Message;
        public int FrameIndex;
    }
}
