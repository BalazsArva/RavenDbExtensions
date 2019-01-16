using System.Linq.Expressions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.Abstractions;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.ExpressionProcessors
{
    public class LambdaExpressionProcessor : IExpressionProcessor
    {
        public bool TryProcess(Expression expression, out string result)
        {
            var lambdaExpression = expression as LambdaExpression;

            if (lambdaExpression == null)
            {
                result = default;

                return false;
            }

            var parameters = lambdaExpression.Parameters;
            var body = lambdaExpression.Body;

            result = ExpressionProcessorPipeline.GetScriptFromConditionExpression(body);

            return true;
        }
    }
}