using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace ApiMongoDB.Models
{
    public sealed class Instrumentor : IDisposable
    {
        public const string ServiceName = "ApiMongoDB";
        public ActivitySource Tracer { get; }
        public Meter Recorder { get; }
        public Counter<long> IncomingRequestCounter { get; }
        public Counter<long> PersonReadersCounter { get; }


        private static readonly Action<ILogger, string, Exception> _loggerError =
                LoggerMessage.Define<string>(LogLevel.Error, new EventId(1, "ERROR"), "{Message}");

        private static readonly Action<ILogger, string, Exception> _loggerInformation =
                LoggerMessage.Define<string>(LogLevel.Information, new EventId(1, "INFORMATION"), "{Message}");

        public Instrumentor()
        {
            var version = typeof(Instrumentor).Assembly.GetName().Version?.ToString();
            Tracer = new ActivitySource(ServiceName, version);
            Recorder = new Meter(ServiceName, version);
            IncomingRequestCounter = Recorder.CreateCounter<long>("ApiMongo.incoming.requests", "requests",
                description: "The number of incoming requests to the backend API");
            PersonReadersCounter = Recorder.CreateCounter<long>("ApiMongo.Person.Gets", "requests",
                description: "The number of times a person was read from the database");
        }

        public void LogMessageError(ILogger logger, string message) => _loggerError(logger, message, null);
        public void LogMessageInformation(ILogger logger, string message) => _loggerInformation(logger, message, null);


        public void Dispose()
        {
            Tracer.Dispose();
            Recorder.Dispose();
        }
    }
}
