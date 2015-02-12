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
using System.Reflection;
using System.Reflection.Emit;

namespace LinqDefer.RuntimeTypes
{
    /// <summary>
    /// Responsible for the creation and caching of intermediate types used during the LinqDefer transformation process.
    /// Intermediate types are created in a dynamically-defined assembly called LinqDeferIntermediateTypeCache.
    /// </summary>
    internal static class IntermediateTypeCache
    {
        private static readonly object SyncRoot = new object();

        private static readonly ModuleBuilder ModuleBuilder;
        private static readonly Dictionary<string, IntermediateType> TypeCache; 

        static IntermediateTypeCache()
        {
            TypeCache = new Dictionary<string, IntermediateType>();

            // create the dynamic assembly and module
            var assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("LinqDeferIntermediateTypeCache"), AssemblyBuilderAccess.RunAndCollect);
            ModuleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");
        }

        private static string GetTypeKey(IEnumerable<Type> fieldTypes)
        {
            return string.Join(":", fieldTypes.Select(t => t.FullName));
        }

        /// <summary>
        /// Gets the full name of the intermediate type cache assembly
        /// </summary>
        public static string AssemblyFullName
        {
            get { return ModuleBuilder.Assembly.FullName; }
        }

        /// <summary>
        /// Gets or creates a usable intermediate type with the given fields.
        /// If such a type already exists in the cache, that type will be returned.
        /// This method is thread-safe.
        /// </summary>
        /// <param name="fieldTypes">The types of fields required by the type</param>
        /// <returns>An IntermediateType wrapping the Type to be used</returns>
        public static IntermediateType GetType(IEnumerable<Type> fieldTypes)
        {
            lock (SyncRoot)
            {
                var typesArray = fieldTypes.ToArray();
                var key = GetTypeKey(typesArray);

                IntermediateType result;
                if (!TypeCache.TryGetValue(key, out result))
                {
                    // type doesn't exist in cache, build it and cache it
                    TypeCache[key] = result = BuildType(typesArray);
                }

                return result;
            }
        }

        private static IntermediateType BuildType(Type[] fieldTypes)
        {
            // define the type on the module
            var numTypes = TypeCache.Count;
            var typeName = string.Format("intermediate_type_{0}", numTypes);
            var typeBuilder = ModuleBuilder.DefineType(typeName, TypeAttributes.Class);

            // define the fields on the type
            var numFields = fieldTypes.Length;
            var fields = new FieldInfo[numFields];
            var names = new string[numFields];
            for (int i = 0; i < numFields; i++)
            {
                var fieldType = fieldTypes[i];
                var fieldName = string.Format("field_{0}", i);
                names[i] = fieldName;
                typeBuilder.DefineField(fieldName, fieldType, FieldAttributes.Public);
            }

            // create the type itself
            var newType = typeBuilder.CreateType();

            // get the runtime fields now (FieldInfo returned by DefineField not suitable)
            for (int i = 0; i < numFields; i++)
            {
                fields[i] = newType.GetField(names[i]);
            }

            var result = new IntermediateType(newType, fields);
            return result;
        }
    }
}