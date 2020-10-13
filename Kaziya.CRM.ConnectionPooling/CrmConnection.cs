using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Tooling.Connector;

namespace Kaziya.CRM.ConnectionPooling
{
    public class CrmConnection : IDisposable
    {
        private readonly CrmServiceClient _service;

        public IOrganizationService Service => _service;

        public DateTime LastUsage { get; internal set; }

        public bool HasError => !string.IsNullOrWhiteSpace(_service.LastCrmError);
        public string ErrorMessage => _service.LastCrmError;

        public CrmConnection(string connectionString)
        {
            _service = new CrmServiceClient(connectionString);
        }

        public void Dispose()
        {
            _service.Dispose();
        }
    }
}