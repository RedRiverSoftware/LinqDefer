//
// LinqDefer - a library which extends LINQ query handling for data access -
// allowing for expressions not otherwise supported by the provider.
//
// Copyright (c) Red River Software Ltd.  All rights reserved.
//
// This source code is made available under the terms of the MIT General License.
//
using System;
using System.Reflection;

namespace LinqDefer.RuntimeTypes
{
    /// <summary>
    /// Wrapper for dynamically-created 'intermediate types' used by LinqDefer
    /// </summary>
    internal class IntermediateType
    {
        /// <summary>
        /// The created type's runtime Type object
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// Fields of the created type
        /// </summary>
        public FieldInfo[] Fields { get; private set; }

        /// <summary>
        /// Creates a new IntermediateType object
        /// </summary>
        /// <param name="type">The type that was created</param>
        /// <param name="fields">The fields of the type</param>
        public IntermediateType(Type type, FieldInfo[] fields)
        {
            Type = type;
            Fields = fields;
        }
    }
}