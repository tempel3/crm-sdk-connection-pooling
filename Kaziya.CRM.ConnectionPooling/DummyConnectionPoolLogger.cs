using System;

namespace Kaziya.CRM.ConnectionPooling
{
    public class DummyConnectionPoolLogger : IConnectionPoolLogger
    {
        public void Debug(string message)
        {
            System.Diagnostics.Debug.WriteLine($"Debug: {message}");
        }

        public void Warn(string message)
        {
            System.Diagnostics.Debug.WriteLine($"Warn: {message}");
        }
    }
}