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

            var value = RuntimeExpressionValueResolver.GetValue(memberExpression);

            // There is a logic in converting common types to JS representations in the constant expression processor. So create a dummy constant expression to convert the value.
            // TODO: Refactor that logic elsewhere so we don't have to create a dummy constant expression.
            result = ExpressionProcessorPipeline.GetScriptFromConditionExpression(Expression.Constant(value));

            return true;
        }
    }
}