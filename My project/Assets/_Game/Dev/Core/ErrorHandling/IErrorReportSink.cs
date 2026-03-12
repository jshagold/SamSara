namespace Core.ErrorHandling
{
    /// <summary>
    /// Error report transmission/buffer contract (FR-08). Interceptor calls; Transmission implements.
    /// </summary>
    public interface IErrorReportSink
    {
        void Report(ErrorReport report);
    }
}
