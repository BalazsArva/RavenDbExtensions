using System;
using System.Linq.Expressions;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.Visitors
{
    public class ParameterRenamerExpressionVisitor : ExpressionVisitor
    {
        private readonly string _newName;
        private readonly ParameterExpression _originalExpressionIdentity;

        public ParameterRenamerExpressionVisitor(string newName, ParameterExpression originalExpressionIdentity)
        {
            if (string.IsNullOrWhiteSpace(newName))
            {
                throw new ArgumentException($"The parameter '{nameof(newName)}' cannot be null, empty or whitespace-only.", nameof(newName));
            }

            _newName = newName;
            _originalExpressionIdentity = originalExpressionIdentity ?? throw new ArgumentNullException(nameof(originalExpressionIdentity));
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (node == _originalExpressionIdentity)
            {
                return Expression.Parameter(node.Type, _newName);
            }

            return node;
        }
    }
}