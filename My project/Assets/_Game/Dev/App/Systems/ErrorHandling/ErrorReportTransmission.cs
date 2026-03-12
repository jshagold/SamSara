using System;
using System.IO;
using Cysharp.Threading.Tasks;
using Core.ErrorHandling;
using Newtonsoft.Json;
using UnityEngine;

namespace App.Systems.ErrorHandling
{
    /// <summary>
    /// Implements IErrorReportSink: send to remote (stub), on failure buffer to disk; retry on launch (FR-08).
    /// </summary>
    public class ErrorReportTransmission : IErrorReportSink
    {
        private static readonly string BufferDirectory = "error_reports";

        public void Report(ErrorReport report)
        {
            ReportAsync(report).Forget();
        }

        private async UniTaskVoid ReportAsync(ErrorReport report)
        {
            try
            {
                bool sent = await TrySendToRemoteAsync(report);
                if (sent) return;

                string path = GetBufferDirectory();
                if (string.IsNullOrEmpty(path)) return;

                Directory.CreateDirectory(path);
                string filePath = Path.Combine(path, $"report_{report.SnapshotId}_{report.TimestampUtc}.json");
                string json = JsonConvert.SerializeObject(report);
                await UniTask.RunOnThreadPool(() =>
                {
                    File.WriteAllText(filePath, json);
                });
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[ErrorReportTransmission] Buffer write failed: {e.Message}");
            }
        }

        /// <summary>
        /// Call on app launch to retry sending buffered reports; delete on success (L1).
        /// </summary>
        public async UniTask RetryBufferedReportsAsync()
        {
            string path = GetBufferDirectory();
            if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
                return;

            string[] files = Directory.GetFiles(path, "*.json");
            foreach (string filePath in files)
            {
                try
                {
                    string json = await UniTask.RunOnThreadPool(() => File.ReadAllText(filePath));
                    var report = JsonConvert.DeserializeObject<ErrorReport>(json);
                    if (report == null) continue;

                    bool sent = await TrySendToRemoteAsync(report);
                    if (sent)
                    {
                        await UniTask.RunOnThreadPool(() =>
                        {
                            try { File.Delete(filePath); } catch { /* ignore */ }
                        });
                    }
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"[ErrorReportTransmission] Retry failed for {filePath}: {e.Message}");
                }
            }
        }

        private static UniTask<bool> TrySendToRemoteAsync(ErrorReport report)
        {
            // Stub: no endpoint defined yet
            return UniTask.FromResult(false);
        }

        private static string GetBufferDirectory()
        {
            try
            {
                return Path.Combine(Application.persistentDataPath, BufferDirectory);
            }
            catch
            {
                return null;
            }
        }
    }
}
