using System;
using System.Globalization;
using System.Linq.Expressions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.Abstractions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Utilitites;

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

                if (expressionValue == null)
                {
                    result = JSNullString;
                }
                else if (ObjectHelper.IsStringLike(expressionValue))
                {
                    result = string.Concat('"', expressionValue.ToString().Replace("\"", "\\\""), '"');
                }
                else if (ObjectHelper.IsSignedIntegral(expressionValue))
                {
                    result = Convert.ToInt64(expressionValue).ToString(CultureInfo.InvariantCulture);
                }
                else if (ObjectHelper.IsUnsignedIntegral(expressionValue))
                {
                    result = Convert.ToUInt64(expressionValue).ToString(CultureInfo.InvariantCulture);
                }
                else if (ObjectHelper.IsFixedPointNumber(expressionValue))
                {
                    result = ((decimal)expressionValue).ToString(CultureInfo.InvariantCulture);
                }
                else if (ObjectHelper.IsFloatingPointNumber(expressionValue))
                {
                    result = Convert.ToDouble(expressionValue).ToString(CultureInfo.InvariantCulture);
                }
                else if (ObjectHelper.IsLogical(expressionValue))
                {
                    result = ((bool)expressionValue).ToString().ToLower();
                }
                else
                {
                    throw new NotSupportedException($"Constant expression with value of type '{expressionValue.GetType().FullName}' is not supported.");
                }

                return true;
            }

            result = default;

            return false;
        }
    }
}