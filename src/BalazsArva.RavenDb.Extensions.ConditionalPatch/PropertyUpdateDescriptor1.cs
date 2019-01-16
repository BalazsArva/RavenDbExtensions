using System.Linq.Expressions;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch
{
    public class PropertyUpdateDescriptor<TProperty> : PropertyUpdateDescriptor
    {
        public PropertyUpdateDescriptor(MemberExpression memberSelector, TProperty newValue)
            : base(memberSelector, newValue)
        {
        }
    }
}