using System.Linq.Expressions;
using System.Reflection;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.Abstractions;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.ExpressionSimplifiers
{
    public class MemberExpressionSimplifier : IExpressionSimplifier
    {
        public bool TrySimplifyExpression(Expression expression, out Expression result)
        {
            if (expression is MemberExpression memberExpression)
            {
                var memberAccessTarget = memberExpression.Expression;

                return memberAccessTarget == null
                    ? TrySimplifyStaticMemberAccess(memberExpression, out result)
                    : TrySimplifyInstanceMemberAccess(memberExpression, out result);
            }

            result = default;

            return false;
        }

        private bool TrySimplifyStaticMemberAccess(MemberExpression memberExpression, out Expression result)
        {
            // Unlike in MethodCallExpressionSimplifier, static MemberAccess expressions can only be runtime-resolvable,
            // as they have no arguments and the static access itself is runtime-resolvable.
            var fieldInfo = memberExpression.Member as FieldInfo;
            var propertyInfo = memberExpression.Member as PropertyInfo;

            if (fieldInfo != null)
            {
                result = Expression.Constant(fieldInfo.GetValue(null));

                return true;
            }

            if (propertyInfo != null)
            {
                result = Expression.Constant(propertyInfo.GetValue(null));

                return true;
            }

            result = default;

            return false;
        }

        private bool TrySimplifyInstanceMemberAccess(MemberExpression memberExpression, out Expression result)
        {
            var simplifiedMemberAccessTarget = ExpressionSimplifier.SimplifyExpression(memberExpression.Expression);

            var fieldInfo = memberExpression.Member as FieldInfo;
            var propertyInfo = memberExpression.Member as PropertyInfo;

            if (simplifiedMemberAccessTarget is ConstantExpression constantExpression)
            {
                if (fieldInfo != null)
                {
                    result = Expression.Constant(fieldInfo.GetValue(constantExpression.Value));

                    return true;
                }

                if (propertyInfo != null)
                {
                    result = Expression.Constant(propertyInfo.GetValue(constantExpression.Value));

                    return true;
                }
            }

            result = default;

            return false;
        }
    }
}