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
    public class FormattedName
    {
        public string DisplayName { get; set; }
        public string IndexedName { get; set; }

        public override bool Equals(object other)
        {
            var that = other as FormattedName;
            if (that == null) return false;
            if (this.DisplayName != that.DisplayName) return false;
            if (this.IndexedName != that.IndexedName) return false;

            return true;
        }
    }
}