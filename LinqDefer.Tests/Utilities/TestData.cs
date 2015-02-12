//
// LinqDefer - a library which extends LINQ query handling for data access -
// allowing for expressions not otherwise supported by the provider.
//
// Copyright (c) Red River Software Ltd.  All rights reserved.
//
// This source code is made available under the terms of the MIT General License.
//
namespace LinqDefer.Tests.Utilities
{
    public class TestData
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public TestData(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }

        public static TestData[] Sample =
        {
            new TestData("Aaron", "Aaronson"),
            new TestData("Billy", "Bookcase"),
            new TestData("Claire", "Cheswick"),
            new TestData("Dahlia", "Deville")
        };
    }
}