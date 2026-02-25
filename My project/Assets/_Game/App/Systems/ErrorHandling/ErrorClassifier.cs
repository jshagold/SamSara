using System;
using System.Net.Sockets;
using Core.ErrorHandling;

namespace App.Systems.ErrorHandling
{
    /// <summary>
    /// Classifies exceptions into ErrorSeverity (FR-02). No UnityEngine references.
    /// </summary>
    public class ErrorClassifier
    {
        /// <summary>
        /// If explicitTag is provided, returns it. Otherwise classifies by exception type (fail-safe to CRITICAL).
        /// </summary>
        public ErrorSeverity Classify(Exception exception, ErrorSeverity? explicitTag)
        {
            if (explicitTag.HasValue)
                return explicitTag.Value;

            if (exception == null)
                return ErrorSeverity.CRITICAL;

            var type = exception.GetType();

            if (IsNetworkException(type))
                return ErrorSeverity.NETWORK;
            if (IsCriticalException(type))
                return ErrorSeverity.CRITICAL;
            if (IsWarningException(type))
                return ErrorSeverity.WARNING;

            return ErrorSeverity.CRITICAL;
        }

        private static bool IsNetworkException(Type type)
        {
            return type == typeof(SocketException)
                || type == typeof(TimeoutException)
                || type.FullName?.Contains("NetworkException") == true;
        }

        private static bool IsCriticalException(Type type)
        {
            return type == typeof(NullReferenceException)
                || type == typeof(IndexOutOfRangeException)
                || type == typeof(InvalidOperationException);
        }

        private static bool IsWarningException(Type type)
        {
            return type == typeof(ArgumentException)
                || type == typeof(ArgumentNullException);
        }
    }
}
