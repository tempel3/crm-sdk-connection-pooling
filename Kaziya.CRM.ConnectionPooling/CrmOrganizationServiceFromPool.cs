using System;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Kaziya.CRM.ConnectionPooling
{
    public class CrmOrganizationServiceFromPool : IOrganizationService, IDisposable
    {
        private readonly CrmConnectionPool _pool;

        private CrmConnection _connection;

        public CrmConnection Connection => _connection ?? Connect();

        public CrmOrganizationServiceFromPool(CrmConnectionPool pool)
        {
            _pool = pool;
        }

        public CrmConnection Connect()
        {
            return _connection = _pool.ReserveConnection();
        }

        public Guid Create(Entity entity) => Connection.Service.Create(entity);

        public Entity Retrieve(string entityName, Guid id, ColumnSet columnSet) => Connection.Service.Retrieve(entityName, id, columnSet);

        public void Update(Entity entity) => Connection.Service.Update(entity);

        public void Delete(string entityName, Guid id) => Connection.Service.Delete(entityName, id);

        public OrganizationResponse Execute(OrganizationRequest request) => Connection.Service.Execute(request);

        public void Associate(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities) => Connection.Service.Associate(entityName, entityId, relationship, relatedEntities);

        public void Disassociate(string entityName, Guid entityId, Relationship relationship, EntityReferenceCollection relatedEntities) => Connection.Service.Disassociate(entityName, entityId, relationship, relatedEntities);

        public EntityCollection RetrieveMultiple(QueryBase query) => Connection.Service.RetrieveMultiple(query);

        public void Dispose()
        {
            _pool.ReleaseConnection(_connection);
        }
    }
}