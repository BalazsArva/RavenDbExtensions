﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.Abstractions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Utilitites;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.ExpressionProcessors.MethodCallExpressionProcessors.Linq
{
    public class LinqAnyExtensionMethodCallExpressionProcessor : IExpressionProcessor<MethodCallExpression>
    {
        private static readonly MethodInfo AnyExtensionWithoutPredicate;
        private static readonly MethodInfo AnyExtensionWithPredicate;

        private readonly IExpressionProcessorPipeline _expressionProcessorPipeline;

        static LinqAnyExtensionMethodCallExpressionProcessor()
        {
            var overloadedMethods = typeof(Enumerable)
                .GetMethods(BindingFlags.Static | BindingFlags.Public)
                .Where(m => m.Name == "Any")
                .ToList();

            AnyExtensionWithoutPredicate = overloadedMethods
                .SingleOrDefault(m =>
                {
                    var parameters = m.GetParameters();

                    if (parameters.Length == 1)
                    {
                        var param = parameters[0];

                        var paramType = param.ParameterType;
                        if (paramType.IsGenericType)
                        {
                            paramType = paramType.GetGenericTypeDefinition();
                        }

                        return paramType == typeof(IEnumerable<>);
                    }

                    return false;
                });

            AnyExtensionWithPredicate = overloadedMethods
                .SingleOrDefault(m =>
                {
                    var parameters = m.GetParameters();

                    if (parameters.Length == 2)
                    {
                        var param0 = parameters[0];
                        var param1 = parameters[1];

                        var param0Type = param0.ParameterType;
                        if (param0Type.IsGenericType)
                        {
                            param0Type = param0Type.GetGenericTypeDefinition();
                        }

                        var param1Type = param1.ParameterType;
                        if (param1Type.IsGenericType)
                        {
                            param1Type = param1Type.GetGenericTypeDefinition();
                        }

                        return
                            param0Type == typeof(IEnumerable<>) &&
                            param1Type == typeof(Func<,>) &&

                            // Func< {anything} ,bool>
                            param1.ParameterType.GetGenericArguments().ElementAtOrDefault(1) == typeof(bool);
                    }

                    return false;
                });
        }

        public LinqAnyExtensionMethodCallExpressionProcessor(IExpressionProcessorPipeline expressionProcessorPipeline)
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

            if (TryProcessPredicatelessAny(methodCallExpression, parameters, out result))
            {
                return true;
            }

            if (TryProcessPredicatedAny(methodCallExpression, parameters, out result))
            {
                return true;
            }

            result = default;
            return false;
        }

        private bool TryProcessPredicatelessAny(MethodCallExpression methodCallExpression, ScriptParameterDictionary parameters, out string result)
        {
            var invokedMethod = methodCallExpression.Method;
            if (invokedMethod.IsGenericMethod)
            {
                invokedMethod = invokedMethod.GetGenericMethodDefinition();
            }

            if (invokedMethod == AnyExtensionWithoutPredicate)
            {
                // Since extensins are static, we must pass the first method invocation argument as the expression,
                // not the methodCallExpression.Object (because that is null).
                var ownerExpressionString = _expressionProcessorPipeline.ProcessExpression(methodCallExpression.Arguments[0], parameters);

                if (IsInvokedOnDictionary(methodCallExpression))
                {
                    result = $"Object.keys({ownerExpressionString}).length > 0";
                    return true;
                }

                result = $"{ownerExpressionString}.length > 0";
                return true;
            }

            result = default;
            return false;
        }

        private bool TryProcessPredicatedAny(MethodCallExpression methodCallExpression, ScriptParameterDictionary parameters, out string result)
        {
            var invokedMethod = methodCallExpression.Method;
            if (invokedMethod.IsGenericMethod)
            {
                invokedMethod = invokedMethod.GetGenericMethodDefinition();
            }

            if (invokedMethod == AnyExtensionWithPredicate)
            {
                // Since extensins are static, we must pass the first method invocation argument as the expression,
                // not the methodCallExpression.Object (because that is null).
                var ownerExpressionString = _expressionProcessorPipeline.ProcessExpression(methodCallExpression.Arguments[0], parameters);

                var predicate = (LambdaExpression)methodCallExpression.Arguments[1];
                var jsPredicateFunction = _expressionProcessorPipeline.ProcessExpression(predicate, parameters);

                if (IsInvokedOnDictionary(methodCallExpression))
                {
                    // Ugly but simple. Could write a loop instead so we could terminate early if a match is found.
                    result = $"Object.keys({ownerExpressionString}).map(function(key) {{ return {{ Key: key, Value: {ownerExpressionString}[key] }}; }}).some({jsPredicateFunction})";
                    return true;
                }

                result = $"({ownerExpressionString}).some({jsPredicateFunction})";
                return true;
            }

            result = default;
            return false;
        }

        private bool IsInvokedOnDictionary(MethodCallExpression methodCallExpression)
        {
            var targetCollectionType = methodCallExpression.Arguments[0].Type;

            if (targetCollectionType.IsGenericType)
            {
                targetCollectionType = targetCollectionType.GetGenericTypeDefinition();
            }

            var implementsIDictionary = targetCollectionType.GetInterfaces()
                // TODO: It does not work without this. Find out why. Do not remove this line.
                .Select(i => i.IsGenericType ? i.GetGenericTypeDefinition() : i)
                .Any(i => i == typeof(IDictionary<,>));

            return implementsIDictionary;
        }
    }
}