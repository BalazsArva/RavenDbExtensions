using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.Abstractions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Utilitites;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.ExpressionProcessors.MethodCallExpressionProcessors
{
    public class IntegralTypesMethodCallExpressionProcessor : IExpressionProcessor<MethodCallExpression>
    {
        private const string ToStringMethodName = "ToString";

        private static readonly HashSet<Type> integralTypes = new HashSet<Type>
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

        private readonly IExpressionProcessorPipeline _expressionProcessorPipeline;

        public IntegralTypesMethodCallExpressionProcessor(IExpressionProcessorPipeline expressionProcessorPipeline)
        {
            _expressionProcessorPipeline = expressionProcessorPipeline ?? throw new ArgumentNullException(nameof(expressionProcessorPipeline));
        }

        public bool TryProcess(MethodCallExpression methodCallExpression, ScriptParameterDictionary parameters, out string result)
        {
            if (methodCallExpression == null)
            {
                throw new ArgumentNullException(nameof(methodCallExpression));
            }

            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            if (!integralTypes.Contains(methodCallExpression.Method.DeclaringType))
            {
                result = default;

                return false;
            }

            if (methodCallExpression.Method.Name == ToStringMethodName)
            {
                var ownerExpressionString = _expressionProcessorPipeline.ProcessExpression(methodCallExpression.Object, parameters);

                result = $"{ownerExpressionString}.toString()";
                return true;
            }

            result = default;
            return false;
        }
    }
}