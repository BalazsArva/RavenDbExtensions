using System.Linq.Expressions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.Abstractions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Utilitites;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.ExpressionProcessors
{
    public class ConditionalExpressionProcessor : IExpressionProcessor
    {
        public bool TryProcess(Expression expression, ScriptParameterDictionary parameters, out string result)
        {
            if (expression is ConditionalExpression conditionalExpression)
            {
                var test = ExpressionParser.CreateJsScriptFromExpression(conditionalExpression.Test, parameters);
                var ifTrue = ExpressionParser.CreateJsScriptFromExpression(conditionalExpression.IfTrue, parameters);
                var ifFalse = ExpressionParser.CreateJsScriptFromExpression(conditionalExpression.IfFalse, parameters);

                result = $"({test} ? {ifTrue} : {ifFalse})";

                return true;
            }

            result = default;

            return false;
        }
    }
}