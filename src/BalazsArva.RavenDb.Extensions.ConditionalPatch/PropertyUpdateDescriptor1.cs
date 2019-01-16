using System;
using System.Linq.Expressions;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch
{
    public class PropertyUpdateDescriptor<TObject, TProperty> : PropertyUpdateDescriptor
    {
        public PropertyUpdateDescriptor(Expression<Func<TObject, TProperty>> memberSelector, TProperty newValue)
            : base(memberSelector, newValue)
        {
        }
    }
}