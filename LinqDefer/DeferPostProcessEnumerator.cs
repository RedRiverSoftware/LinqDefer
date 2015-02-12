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
using System.Linq.Expressions;
using LinqDefer.Exceptions;

namespace LinqDefer
{
    /// <summary>
    /// Implementation of generic IEnumerator, which will enumerate an underlying
    /// enumerator, applying a compiled version of a supplied lambda to it.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DeferPostProcessEnumerator<T> : IEnumerator<T>
    {
        private readonly IEnumerator _intermediateEnumerator;
        private readonly Delegate _postProcess;
        private T _current;
        private bool _ended;

        /// <summary>
        /// Creates a new instance of the post-processing enumerator
        /// </summary>
        /// <param name="intermediateEnumerator">Enumerator to be enumerated</param>
        /// <param name="processLambda">Lamba to compile and apply to each enumerated item</param>
        public DeferPostProcessEnumerator(IEnumerator intermediateEnumerator, LambdaExpression processLambda)
        {
            _intermediateEnumerator = intermediateEnumerator;

            _postProcess = processLambda.Compile();
        }

        /// <summary>
        /// Releases all resources associated with the enumerator
        /// </summary>
        public void Dispose()
        {
            var disposableEnumerator = _intermediateEnumerator as IDisposable;
            if (disposableEnumerator != null)
            {
                disposableEnumerator.Dispose();
            }
        }

        /// <summary>
        /// Moves to the next item in the enumeration
        /// </summary>
        /// <returns>'true' if another item is now available via Current; 'false' if we reached the end</returns>
        public bool MoveNext()
        {
            var moved = _intermediateEnumerator.MoveNext();
            if (moved)
            {
                var intermediateItem = _intermediateEnumerator.Current;
                var resultItem = PostProcess(intermediateItem);
                _current = resultItem;
            }
            else
            {
                _ended = true;
            }
            return moved;
        }

        /// <summary>
        /// Restarts enumeration from the beginning
        /// </summary>
        public void Reset()
        {
            _intermediateEnumerator.Reset();
            _ended = false;
            _current = default(T);
        }

        /// <summary>
        /// Retrieves the item at the current point of enumeration
        /// </summary>
        public T Current
        {
            get
            {
                if (_ended) throw new InvalidOperationException();
                return _current;
            }
        }

        /// <summary>
        /// Retrieves the item at the current point of enumeration
        /// </summary>
        object IEnumerator.Current
        {
            get { return Current; }
        }

        private T PostProcess(object intermediateItem)
        {
            T result;
            try
            {
                result = (T)_postProcess.DynamicInvoke(intermediateItem);
            }
            catch (Exception ex)
            {
                var msg = string.Format("Deferred execution of LINQ expression failed: {0}", ex.Message);
                throw new LinqDeferException(msg, ex);
            }
            return result;
        }
    }
}