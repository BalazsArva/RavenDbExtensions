using System.Linq.Expressions;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch
{
    public abstract class PropertyUpdateDescriptor
    {
        protected PropertyUpdateDescriptor(Expression memberSelector, object newValue)
        {
            MemberSelector = memberSelector;
            NewValue = newValue;
        }

        public object NewValue { get; }

        public Expression MemberSelector { get; }
    }
}