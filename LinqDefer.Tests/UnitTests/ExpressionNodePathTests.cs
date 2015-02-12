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
using System.Text;
using System.Threading.Tasks;
using LinqDefer.ExpressionAnalysers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinqDefer.Tests.UnitTests
{
    [TestClass]
    public class ExpressionNodePathTests
    {
        [TestMethod]
        public void Empty_node_path_yields_null_current()
        {
            var path = new ExpressionNodePath();
            Assert.IsNull(path.Current);
        }
        
        [TestMethod]
        public void Two_node_path_parents_are_correct()
        {
            var path = new ExpressionNodePath();
            var rootExp = new ClassifiedExpression(null);
            var childExp = new ClassifiedExpression(null);

            path.Insert(0, rootExp);
            Assert.AreEqual(null, path.Parent);

            path.Insert(0, childExp);
            Assert.AreEqual(rootExp, path.Parent);
        }
    }
}
