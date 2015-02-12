using System;
using System.Linq;
using LinqDefer.Sample.EfModel;
using System.Data.Entity.Migrations;

namespace LinqDefer.Sample.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<SampleContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(SampleContext context)
        {
            // migration seed
            // (see SeedInitialiser for initial seed)
        }
    }
}
