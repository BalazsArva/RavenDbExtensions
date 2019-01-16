﻿using System.Linq.Expressions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.Abstractions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Utilitites;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.ExpressionProcessors
{
    public class ParameterBoundMemberExpressionProcessor : IExpressionProcessor
    {
        public bool TryProcess(Expression expression, ScriptParameterDictionary parameters, out string result)
        {
            if (!(expression is MemberExpression memberExpression) || !ExpressionHelper.IsParameterBoundExpression(memberExpression))
            {
                result = default;

                return false;
            }

            var ownerExpressionString = ExpressionParser.CreateJsScriptFromExpression(memberExpression.Expression, parameters);
            var member = memberExpression.Member;

            // TODO: Consider cases when the property is called differently in the document than in the object model. Check out
            // Raven.Client.Documents.Linq.LinqPathProvider, maybe that can solve it out-of-the-box.
            result = $"{ownerExpressionString}.{member.Name}";

            return true;
        }
    }
}