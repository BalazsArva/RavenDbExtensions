using System.Linq.Expressions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.Abstractions;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.ExpressionProcessors
{
    public class ConditionalExpressionProcessor : IExpressionProcessor
    {
        public bool TryProcess(Expression expression, out string result)
        {
            if (expression is ConditionalExpression conditionalExpression)
            {
                var test = ExpressionProcessorPipeline.GetScriptFromConditionExpression(conditionalExpression.Test);
                var ifTrue = ExpressionProcessorPipeline.GetScriptFromConditionExpression(conditionalExpression.IfTrue);
                var ifFalse = ExpressionProcessorPipeline.GetScriptFromConditionExpression(conditionalExpression.IfFalse);

                result = $"({test} ? {ifTrue} : {ifFalse})";

                return true;
            }

            result = default;

            return false;
        }
    }
}