//
// LinqDefer - a library which extends LINQ query handling for data access -
// allowing for expressions not otherwise supported by the provider.
//
// Copyright (c) Red River Software Ltd.  All rights reserved.
//
// This source code is made available under the terms of the MIT General License.
//
using System;

namespace LinqDefer.Tests.Utilities
{
    public static class ExceptionThrower
    {
        public static object Throw()
        {
            throw new Exception();
        }
    }
}