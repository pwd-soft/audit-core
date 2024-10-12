using System.Threading.Tasks;

namespace PWD.Audit.Data
{
    public interface IAuditDbSchemaMigrator
    {
        Task MigrateAsync();
    }
}
