using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace PWD.Audit.Data
{
    /* This is used if database provider does't define
     * ILoggerDbSchemaMigrator implementation.
     */
    public class NullLoggerDbSchemaMigrator : IAuditDbSchemaMigrator, ITransientDependency
    {
        public Task MigrateAsync()
        {
            return Task.CompletedTask;
        }
    }
}