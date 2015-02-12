//
// LinqDefer - a library which extends LINQ query handling for data access -
// allowing for expressions not otherwise supported by the provider.
//
// Copyright (c) Red River Software Ltd.  All rights reserved.
//
// This source code is made available under the terms of the MIT General License.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinqDefer.Sample.EfModel;

namespace LinqDefer.Sample
{
    /// <summary>
    /// Simple LinqDefer sample
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            // create our sample DB context.  it has a custom logger to show the underlying SQL statement
            var context = new SampleContext();

            // now we run a query - note, without "ThenDoDeferred", this would throw an exception since
            // EntityFramework doesn't know what to do with string.Format.

            // also note the SQL statement logged to console: it contains just the fields needed to
            // build our result.
            var names = context.People
                .Where(p => p.LastName.StartsWith("a"))
                .Take(2)
                .Select(p => string.Format("{0}, {1}", p.LastName.ToUpper(), p.FirstName))
                .ThenDoDeferred();

            // write out results
            foreach (var name in names)
            {
                Console.WriteLine(name);
            }

            Console.WriteLine();
            Console.WriteLine("Hit enter to exit");
            Console.ReadLine();
        }
    }
}
