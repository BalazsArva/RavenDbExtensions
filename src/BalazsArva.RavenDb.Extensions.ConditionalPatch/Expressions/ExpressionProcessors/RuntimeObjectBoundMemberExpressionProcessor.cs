using System.Linq.Expressions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.Abstractions;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.ExpressionProcessors
{
    public class RuntimeObjectBoundMemberExpressionProcessor : IExpressionProcessor
    {
        public bool TryProcess(Expression expression, out string result)
        {
            if (!(expression is MemberExpression memberExpression) || !ExpressionHelper.IsRuntimeObjectBoundExpression(memberExpression))
            {
                result = default;

                return false;
            }

            var expressionValue = RuntimeExpressionValueResolver.GetValue(memberExpression);

            result = ConstantValueConverter.ConvertToJson(expressionValue);

            return true;
        }
    }
}