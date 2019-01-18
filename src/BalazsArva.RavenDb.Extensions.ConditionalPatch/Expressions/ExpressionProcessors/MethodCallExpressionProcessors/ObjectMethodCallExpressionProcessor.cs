using System;
using System.Linq.Expressions;
using System.Reflection;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.Abstractions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Utilitites;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.ExpressionProcessors.MethodCallExpressionProcessors
{
    public class ObjectMethodCallExpressionProcessor : IExpressionProcessor
    {
        private static readonly Type objectType = typeof(object);

        private static readonly MethodInfo NonStatic_ToString = objectType.GetMethod("ToString", Array.Empty<Type>());

        public bool TryProcess(Expression expression, ScriptParameterDictionary parameters, out string result)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            var methodCallExpression = expression as MethodCallExpression;
            if (methodCallExpression == null || methodCallExpression.Method.DeclaringType != objectType)
            {
                result = default;

                return false;
            }

            if (methodCallExpression.Method == NonStatic_ToString)
            {
                var ownerExpressionString = ExpressionParser.CreateJsScriptFromExpression(methodCallExpression.Object, parameters);

                result = $"{ownerExpressionString}.toString()";
                return true;
            }

            result = default;
            return false;
        }
    }
}