using System;
using System.IO;
using Microsoft.Xrm.Sdk.Query;

namespace Kaziya.CRM.ConnectionPooling.Console
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            DotNetEnv.Env.Load();
            var connectionString = System.Environment.GetEnvironmentVariable("ConnectionString");
            
            var pool = new CrmConnectionPool(connectionString);

            using (var service = new CrmOrganizationServiceFromPool(pool))
            {
                var query = new QueryExpression("account")
                {
                    NoLock = true,
                    TopCount = 2
                };

                var accounts = service.RetrieveMultiple(query);
                
                System.Console.WriteLine($"Accounts: {accounts.Entities.Count}");
            }
        }
    }
}