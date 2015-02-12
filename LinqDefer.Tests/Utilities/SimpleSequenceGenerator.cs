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
    public class SimpleSequenceGenerator
    {
        private int _next = 0;

        public int GetNextA()
        {
            return _next++;
        }

        public int GetNextB()
        {
            return _next++;
        }
    }
}
