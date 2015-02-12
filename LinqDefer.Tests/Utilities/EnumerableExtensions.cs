//
// LinqDefer - a library which extends LINQ query handling for data access -
// allowing for expressions not otherwise supported by the provider.
//
// Copyright (c) Red River Software Ltd.  All rights reserved.
//
// This source code is made available under the terms of the MIT General License.
//
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinqDefer.Tests.Utilities
{
    public static class EnumerableExtensions
    {
        public static Array ToArray(this IEnumerator enumerable)
        {
            var results = new ArrayList();
            while (enumerable.MoveNext())
            {
                results.Add(enumerable.Current);
            }
            return results.ToArray();
        }
    }
}
