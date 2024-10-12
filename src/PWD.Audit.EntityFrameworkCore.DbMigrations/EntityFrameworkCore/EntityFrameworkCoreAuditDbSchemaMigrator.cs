using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PWD.Audit.Data;
using Volo.Abp.DependencyInjection;

namespace PWD.Audit.EntityFrameworkCore
{
    public class EntityFrameworkCoreAuditDbSchemaMigrator
        : IAuditDbSchemaMigrator, ITransientDependency
    {
        private readonly IServiceProvider _serviceProvider;

        public EntityFrameworkCoreAuditDbSchemaMigrator(
            IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task MigrateAsync()
        {
            /* We intentionally resolving theAuditMigrationsDbContext
             * from IServiceProvider (instead of directly injecting it)
             * to properly get the connection string of the current tenant in the
             * current scope.
             */

            await _serviceProvider
                .GetRequiredService<AuditMigrationsDbContext>()
                .Database
                .MigrateAsync();
        }
    }
}