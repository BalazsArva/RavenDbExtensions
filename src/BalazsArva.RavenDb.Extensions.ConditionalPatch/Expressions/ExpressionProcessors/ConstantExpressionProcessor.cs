using System.Linq.Expressions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.Abstractions;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.ExpressionProcessors
{
    public class ConstantExpressionProcessor : IExpressionProcessor
    {
        private const string JSNullString = "null";

        public bool TryProcess(Expression expression, out string result)
        {
            if (expression is ConstantExpression constantExpression)
            {
                var expressionValue = constantExpression.Value;

                result = ConstantValueConverter.ConvertToJson(expressionValue);

                return true;
            }

            result = default;

            return false;
        }
    }
}