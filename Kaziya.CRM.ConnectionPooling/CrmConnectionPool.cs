using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Kaziya.CRM.ConnectionPooling
{
    public class CrmConnectionPool
    {
        private readonly string _connectionString;
        private readonly IList<CrmConnection> _connections = new List<CrmConnection>();
        private readonly Stack<CrmConnection> _stack = new Stack<CrmConnection>();
        private readonly object _syncLock = new object();
        
        public IConnectionPoolLogger Logger { get; set;  } = new DummyConnectionPoolLogger();

        public int MaxConnections { get; set; } = 32;

        public TimeSpan TimeToLive { get; set; } = TimeSpan.FromMinutes(10);

        public CrmConnectionPool(string connectionString)
        {
            _connectionString = connectionString;
            Logger.Debug("CrmConnectionPool.ctor");
        }

        private void IncreaseByOneConnection()
        {
            int count;
            lock (_syncLock)
            {
                var connection = new CrmConnection(_connectionString);
                _connections.Add(connection);
                _stack.Push(connection);

                count = _connections.Count;
            }

            Logger.Debug($"CrmConnectionPool.IncreaseByOneConnection to {count}.");
        }

        private void DecreaseByOneConnection()
        {
            int count = -1;
            lock (_syncLock)
            {
                if (_stack.Any())
                {
                    var connection = _stack.Pop();
                    _connections.Remove(connection);
                    connection.Dispose();

                    count = _connections.Count;
                }
            }

            if (count == -1)
            {
                Logger.Warn($"CrmConnectionPool.DecreaseByOneConnection no connection found.");
            }
            else
            {
                Logger.Debug($"CrmConnectionPool.DecreaseByOneConnection to {count}.");
            }
        }

        private int RemoveConnectionsFromConnections(CrmConnection connection)
        {
            _connections.Remove(connection);
            connection.Dispose();
            return _connections.Count;
        }

        public CrmConnection ReserveConnection()
        {
            while (true)
            {
                var nextAvailable = GetNextAvailable();
                if (nextAvailable != null)
                    return nextAvailable;

                Logger.Warn($"CrmConnectionPool.ReserveConnection has no free connection and has to wait.");

                // stale
                Thread.Sleep(1000);
            }
        }

        private CrmConnection GetNextAvailable()
        {
            lock (_syncLock)
            {
                // scale down
                if (_stack.Count > 1 && _stack.Any(i => i.LastUsage < DateTime.Now - TimeToLive))
                {
                    DecreaseByOneConnection();
                }

                // use available
                if (_stack.Any())
                {
                    return DequeueAndUpdateLastUsage();
                }

                // scale up
                if (!_stack.Any() && _connections.Count < MaxConnections)
                {
                    IncreaseByOneConnection();
                    return DequeueAndUpdateLastUsage();
                }
            }

            return null;
        }

        private CrmConnection DequeueAndUpdateLastUsage()
        {
            var item = _stack.Pop();
            item.LastUsage = DateTime.Now;

            Logger.Debug($"CrmConnectionPool.DequeueAndUpdateLastUsage Connections: {_connections.Count} Queue: {_stack.Count}.");

            return item;
        }

        public void ReleaseConnection(CrmConnection connection)
        {
            if (connection == null)
                return;

            lock (_syncLock)
            {
                if (connection.HasError)
                {
                    RemoveConnectionsFromConnections(connection);
                    Logger.Debug($"CrmConnectionPool.ReleaseConnection Removed due to error: {connection.ErrorMessage}.");
                }
                else
                {
                    _stack.Push(connection);
                }

                Logger.Debug($"CrmConnectionPool.ReleaseConnection Connections: {_connections.Count} Queue: {_stack.Count}.");
            }
        }
    }
}
