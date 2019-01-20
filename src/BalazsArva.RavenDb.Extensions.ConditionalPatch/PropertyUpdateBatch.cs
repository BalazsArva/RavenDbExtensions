using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch
{
    public class PropertyUpdateBatch<TDocument>
    {
        private readonly List<PropertyUpdateDescriptor> propertyUpdates = new List<PropertyUpdateDescriptor>();

        public PropertyUpdateBatch()
        {
        }

        public PropertyUpdateBatch<TDocument> Add<TProperty>(Expression<Func<TDocument, TProperty>> memberSelector, TProperty newValue)
        {
            if (memberSelector.Body is MemberExpression memberExpression)
            {
                propertyUpdates.Add(new PropertyUpdateDescriptor<TProperty>(memberExpression, newValue));

                return this;
            }

            throw new ArgumentException($"The body of the parameter '{nameof(memberSelector)}' must be a {nameof(MemberExpression)}.", nameof(memberSelector));
        }

        public PropertyUpdateDescriptor[] CreateBatch()
        {
            return propertyUpdates.ToArray();
        }
    }
}