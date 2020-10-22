using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace CommonLib.Models.Linq
{
    internal class DynamicOrdering
    {
        public Expression Selector;
        public bool Ascending;
    }
}
