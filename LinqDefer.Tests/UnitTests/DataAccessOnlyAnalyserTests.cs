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
using System.Linq.Expressions;
using LinqDefer.ExpressionAnalysers;
using LinqDefer.ExpressionAnalysers.DataAccessOnly;
using LinqDefer.Tests.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinqDefer.Tests.UnitTests
{
    [TestClass]
    public class DataAccessOnlyAnalyserTests
    {
        [TestMethod]
        public void Identifies_source_parameter_as_valid_expression()
        {
            var dao = new DataAccessOnlyAnalyser();
            var enp = new ExpressionNodePath();

            Expression<Func<string, string>> testLambda = ((string a) => a);
            dao.Initialise(testLambda.Parameters[0]);
            enp.Add(new ClassifiedExpression(testLambda.Body));

            Assert.IsTrue(dao.ShouldProviderHandleExpression(enp));
        }

        [TestMethod]
        public void Identifies_constant_as_invalid_expression()
        {
            var dao = new DataAccessOnlyAnalyser();
            var enp = new ExpressionNodePath();

            Expression<Func<string, int>> testLambda = ((string a) => 1);
            dao.Initialise(testLambda.Parameters[0]);
            enp.Add(new ClassifiedExpression(testLambda.Body));

            Assert.IsFalse(dao.ShouldProviderHandleExpression(enp));
        }

        [TestMethod]
        public void Identifies_field_access_as_valid_expression()
        {
            var dao = new DataAccessOnlyAnalyser();
            var enp = new ExpressionNodePath();

            Expression<Func<ClassWithField, long>> testLambda = ((ClassWithField a) => a.Test);
            dao.Initialise(testLambda.Parameters[0]);
            enp.Add(new ClassifiedExpression(testLambda.Body));

            Assert.IsTrue(dao.ShouldProviderHandleExpression(enp));
        }

        [TestMethod]
        public void Identifies_property_access_as_valid_expression()
        {
            var dao = new DataAccessOnlyAnalyser();
            var enp = new ExpressionNodePath();

            Expression<Func<DateTime, long>> testLambda = ((DateTime a) => a.Ticks);
            dao.Initialise(testLambda.Parameters[0]);
            enp.Add(new ClassifiedExpression(testLambda.Body));

            Assert.IsTrue(dao.ShouldProviderHandleExpression(enp));
        }

        [TestMethod]
        public void Identifies_binary_operation_as_invalid_expression()
        {
            var dao = new DataAccessOnlyAnalyser();
            var enp = new ExpressionNodePath();

            Expression<Func<DateTime, long>> testLambda = ((DateTime a) => a.Ticks + 9);
            dao.Initialise(testLambda.Parameters[0]);
            enp.Add(new ClassifiedExpression(testLambda.Body));

            Assert.IsFalse(dao.ShouldProviderHandleExpression(enp));
        }

        [TestMethod]
        public void Identifies_data_based_array_index_as_valid_expression()
        {
            var dao = new DataAccessOnlyAnalyser();
            var enp = new ExpressionNodePath();

            Expression<Func<string[], string>> testLambda = ((string[] a) => a[a.GetHashCode()]);
            dao.Initialise(testLambda.Parameters[0]);
            enp.Add(new ClassifiedExpression(testLambda.Body));

            Assert.IsTrue(dao.ShouldProviderHandleExpression(enp));
        }

        [TestMethod]
        public void Identifies_non_data_based_array_as_invalid_expression()
        {
            var dao = new DataAccessOnlyAnalyser();
            var enp = new ExpressionNodePath();

            Expression<Func<string, char>> testLambda = ((string a) => DateTime.Now.ToString()[a.GetHashCode()]);
            dao.Initialise(testLambda.Parameters[0]);
            enp.Add(new ClassifiedExpression(testLambda.Body));

            Assert.IsFalse(dao.ShouldProviderHandleExpression(enp));
        }

        [TestMethod]
        public void Identifies_array_index_as_valid_expression()
        {
            var dao = new DataAccessOnlyAnalyser();
            var enp = new ExpressionNodePath();

            Expression<Func<string[], string>> testLambda = ((string[] a) => a[0]);
            dao.Initialise(testLambda.Parameters[0]);
            enp.Add(new ClassifiedExpression(testLambda.Body));

            Assert.IsTrue(dao.ShouldProviderHandleExpression(enp));
        }

        [TestMethod]
        public void Identifies_non_constant_array_index_as_invalid_expression()
        {
            var dao = new DataAccessOnlyAnalyser();
            var enp = new ExpressionNodePath();

            Expression<Func<string[], string>> testLambda = ((string[] a) => a[DateTime.Now.Ticks]);
            dao.Initialise(testLambda.Parameters[0]);
            enp.Add(new ClassifiedExpression(testLambda.Body));

            Assert.IsFalse(dao.ShouldProviderHandleExpression(enp));
        }

        [TestMethod]
        public void Identifies_object_method_call_as_valid_expression()
        {
            var dao = new DataAccessOnlyAnalyser();
            var enp = new ExpressionNodePath();

            Expression<Func<string, string>> testLambda = ((string a) => a.ToString());
            dao.Initialise(testLambda.Parameters[0]);
            enp.Add(new ClassifiedExpression(testLambda.Body));

            Assert.IsTrue(dao.ShouldProviderHandleExpression(enp));
        }

        [TestMethod]
        public void Identifies_member_access_and_indexer_as_valid_expression()
        {
            var dao = new DataAccessOnlyAnalyser();
            var enp = new ExpressionNodePath();

            Expression<Func<string, char>> testLambda = ((string a) => a.ToString()[0]);
            dao.Initialise(testLambda.Parameters[0]);
            enp.Add(new ClassifiedExpression(testLambda.Body));

            Assert.IsTrue(dao.ShouldProviderHandleExpression(enp));
        }

        [TestMethod]
        public void Identifies_extension_method_call_as_invalid_expression()
        {
            var dao = new DataAccessOnlyAnalyser();
            var enp = new ExpressionNodePath();

            Expression<Func<string, IEnumerable<char>>> testLambda = ((string a) => a.Reverse());
            dao.Initialise(testLambda.Parameters[0]);
            enp.Add(new ClassifiedExpression(testLambda.Body));

            Assert.IsFalse(dao.ShouldProviderHandleExpression(enp));
        }
    }
}
