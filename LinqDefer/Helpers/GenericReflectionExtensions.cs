//
// LinqDefer - a library which extends LINQ query handling for data access -
// allowing for expressions not otherwise supported by the provider.
//
// Copyright (c) Red River Software Ltd.  All rights reserved.
//
// This source code is made available under the terms of the MIT General License.
//
using System;
using System.Linq;
using System.Reflection;

namespace LinqDefer.Helpers
{
    /// <summary>
    /// Reflection extensions for working with generic types and methods
    /// </summary>
    internal static class GenericReflectionExtensions
    {
        /// <summary>
        /// Finds a generic method for the given method name, binding flags, type arguments and parameter types.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="methodName"></param>
        /// <param name="flags"></param>
        /// <param name="typeArguments"></param>
        /// <param name="parameterTypes"></param>
        /// <returns></returns>
        public static MethodInfo GetGenericMethod(this Type type, string methodName, BindingFlags flags, Type[] typeArguments,
            Type[] parameterTypes)
        {
            // first find methods where the name matches and they're generic
            var allMethods = type.GetMethods(flags);
            var matchingMethods = allMethods.Where(m => m.Name == methodName && m.IsGenericMethodDefinition).ToList();
            if (!matchingMethods.Any())
            {
                throw new InvalidOperationException("Generic method not found at all");
            }

            // now create actual generic method handles and check parameters
            var methodsWithCorrectParameters = matchingMethods.Where(m =>
            {
                var genericMethod = m.MakeGenericMethod(typeArguments);
                var parameters = genericMethod.GetParameters();

                if (parameters.Length != parameterTypes.Length) return false;
                var noMismatches = !parameters.Where((t, i) => t.ParameterType != parameterTypes[i]).Any();

                return noMismatches;
            }).ToList();

            // find the single resulting method
            if (methodsWithCorrectParameters.Count() != 1)
            {
                throw new InvalidOperationException("No single generic method found with correct parameter types");
            }
            var matchingMethodDefinition = methodsWithCorrectParameters.Single();
            var actualMethodFromDefinition = matchingMethodDefinition.MakeGenericMethod(typeArguments);

            return actualMethodFromDefinition;
        }
    }
}