using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.Abstractions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Utilitites;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.ExpressionProcessors.MethodCallExpressionProcessors
{
    public class ParameterBoundMethodCallExpressionProcessor : IExpressionProcessor
    {
        private static readonly HashSet<Type> IntegralTypes = new HashSet<Type>
        {
            typeof(sbyte),
            typeof(byte),
            typeof(ushort),
            typeof(short),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong)
        };

        private readonly IEnumerable<IExpressionProcessor> expressionProcessors;

        public ParameterBoundMethodCallExpressionProcessor()
        {
            expressionProcessors = new List<IExpressionProcessor>
            {
                new ObjectMethodCallExpressionProcessor(),
                new StringMethodCallExpressionProcessor()
            };
        }

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
            if (methodCallExpression == null)
            {
                result = default;

                return false;
            }

            foreach (var processor in expressionProcessors)
            {
                if (processor.TryProcess(expression, parameters, out result))
                {
                    return true;
                }
            }

            result = default;

            return false;
        }
    }
}