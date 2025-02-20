﻿using System.Diagnostics;

namespace ApiMongoDB.Models
{
    /// <summary>
    /// It is recommended to use a custom type to hold references for ActivitySource.
    /// This avoids possible type collisions with other components in the DI container.
    /// </summary>
    public class Instrumentation : IDisposable
    {
        internal const string ActivitySourceName = "Tracing.ApiMongoDB";
        internal const string ActivitySourceVersion = "1.0.0";

        public ActivitySource ActivitySource { get; }

        public Instrumentation()
        {
            this.ActivitySource = new ActivitySource(ActivitySourceName, ActivitySourceVersion);
        }

        public void Dispose()
        {
            this.ActivitySource.Dispose();
        }
    }
}
