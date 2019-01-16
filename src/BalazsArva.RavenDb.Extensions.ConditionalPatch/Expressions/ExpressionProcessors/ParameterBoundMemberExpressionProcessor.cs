using System.Linq.Expressions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.Abstractions;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.ExpressionProcessors
{
    public class ParameterBoundMemberExpressionProcessor : IExpressionProcessor
    {
        public bool TryProcess(Expression expression, out string result)
        {
            if (!(expression is MemberExpression memberExpression) || !ExpressionHelper.IsParameterBoundExpression(memberExpression))
            {
                result = default;

                return false;
            }

            var ownerExpressionString = ExpressionProcessorPipeline.GetScriptFromConditionExpression(memberExpression.Expression);
            var member = memberExpression.Member;

            result = $"{ownerExpressionString}.{member.Name}";

            return true;
        }
    }
}