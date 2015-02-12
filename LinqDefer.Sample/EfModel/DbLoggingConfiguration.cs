using System.Data.Entity;

namespace LinqDefer.Sample.EfModel
{
    public class DbLoggingConfiguration : DbConfiguration
    {
        public DbLoggingConfiguration()
        {
            SetDatabaseLogFormatter((ctx, action) => new CommandOnlyLogger(ctx, action));
        }
    }
}