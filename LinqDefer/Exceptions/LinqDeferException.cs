//
// LinqDefer - a library which extends LINQ query handling for data access -
// allowing for expressions not otherwise supported by the provider.
//
// Copyright (c) Red River Software Ltd.  All rights reserved.
//
// This source code is made available under the terms of the MIT General License.
//
using System;

namespace LinqDefer.Exceptions
{
    /// <summary>
    /// An exception thrown by the LinqDefer library
    /// </summary>
    public class LinqDeferException : Exception
    {
        /// <summary>
        /// Creates a new LinqDeferException instance
        /// </summary>
        /// <param name="message">Message describing the fault</param>
        /// <param name="innerException">Inner cause of the fault</param>
        public LinqDeferException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Creates a new LinqDeferException instance
        /// </summary>
        /// <param name="message">Message describing the fault</param>
        public LinqDeferException(string message)
            : base(message)
        {
        }
    }
}