﻿//
// LinqDefer - a library which extends LINQ query handling for data access -
// allowing for expressions not otherwise supported by the provider.
//
// Copyright (c) Red River Software Ltd.  All rights reserved.
//
// This source code is made available under the terms of the MIT General License.
//
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinqDefer.Sample.Migrations;

namespace LinqDefer.Sample.EfModel
{
    public class SampleContext : DbContext
    {
        public IDbSet<Person> People { get; set; }

        public SampleContext() : base("LinqDeferSample")
        {
            Database.Log = Console.Write;
            Database.SetInitializer(new SeedInitialiser());
        }
    }
}
