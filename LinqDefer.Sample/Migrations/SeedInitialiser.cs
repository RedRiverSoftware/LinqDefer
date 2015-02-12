using System.Data.Entity;
using LinqDefer.Sample.EfModel;

namespace LinqDefer.Sample.Migrations
{
    public class SeedInitialiser : CreateDatabaseIfNotExists<SampleContext>
    {
        protected override void Seed(SampleContext context)
        {
            // initial seed
            context.People.Add(new Person { FirstName = "Aaron", LastName = "Aaronson" });
            context.People.Add(new Person { FirstName = "Nicholas", LastName = "Angel" });
            context.People.Add(new Person { FirstName = "Danny", LastName = "Butterman" });
            context.SaveChanges();
        }
    }
}