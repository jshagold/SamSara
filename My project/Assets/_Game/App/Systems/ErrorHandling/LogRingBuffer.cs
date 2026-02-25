using System;
using Core.ErrorHandling;
using UnityEngine;

namespace App.Systems.ErrorHandling
{
    /// <summary>
    /// Fixed-size ring buffer for log context (FR-01). Non-allocating append; GetSnapshot() copies to new array.
    /// </summary>
    public class LogRingBuffer
    {
        public const int Capacity = 100;

        private readonly LogRingBufferEntry[] _entries;
        private int _head;
        private int _count;

        public LogRingBuffer()
        {
            _entries = new LogRingBufferEntry[Capacity];
            _head = 0;
            _count = 0;
        }

        public void Append(LogType logType, string message, int frameIndex)
        {
            var entry = new LogRingBufferEntry
            {
                LogType = logType,
                Message = message ?? string.Empty,
                FrameIndex = frameIndex
            };
            _entries[_head] = entry;
            _head = (_head + 1) % Capacity;
            if (_count < Capacity)
                _count++;
        }

        /// <summary>
        /// Returns a copy of current entries for StateSnapshot.RingBufferPayload (M2).
        /// </summary>
        public LogRingBufferEntry[] GetSnapshot()
        {
            if (_count == 0)
                return Array.Empty<LogRingBufferEntry>();

            var result = new LogRingBufferEntry[_count];
            int start = _count < Capacity ? 0 : _head;
            for (int i = 0; i < _count; i++)
            {
                int index = (start + i) % Capacity;
                result[i] = _entries[index];
            }
            return result;
        }
    }
}
