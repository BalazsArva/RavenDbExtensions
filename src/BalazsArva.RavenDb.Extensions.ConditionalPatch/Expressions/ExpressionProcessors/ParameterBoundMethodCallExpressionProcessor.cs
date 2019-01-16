using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.Abstractions;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.ExpressionProcessors
{
    public class ParameterBoundMethodCallExpressionProcessor : IExpressionProcessor
    {
        private static readonly Type ObjectType = typeof(object);
        private static readonly Type StringType = typeof(string);

        private static readonly MethodInfo String_ToLower = typeof(string).GetMethod("ToLower", Array.Empty<Type>());
        private static readonly MethodInfo String_ToUpper = typeof(string).GetMethod("ToUpper", Array.Empty<Type>());

        private static readonly MethodInfo String_Trim = typeof(string).GetMethod("Trim", Array.Empty<Type>());
        private static readonly MethodInfo String_TrimStart = typeof(string).GetMethod("TrimStart", Array.Empty<Type>());
        private static readonly MethodInfo String_TrimEnd = typeof(string).GetMethod("TrimEnd", Array.Empty<Type>());

        private static readonly MethodInfo String_Substring_StartIndex = typeof(string).GetMethod("Substring", new[] { typeof(int) });
        private static readonly MethodInfo String_Substring_StartIndex_Length = typeof(string).GetMethod("Substring", new[] { typeof(int), typeof(int) });

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

        public bool TryProcess(Expression expression, out string result)
        {
            if (!(expression is MethodCallExpression methodCallExpression) || !ExpressionHelper.IsParameterBoundExpression(methodCallExpression))
            {
                result = default;

                return false;
            }

            string mappedMethodName = null;
            var method = methodCallExpression.Method;

            if (method.DeclaringType == ObjectType)
            {
                switch (method.Name)
                {
                    case "ToString":
                        mappedMethodName = "toString";
                        break;
                }
            }
            else if (method.DeclaringType == StringType)
            {
                result = ProcessStringMethodInvocation(methodCallExpression, method);

                return true;
            }
            else if (IntegralTypes.Contains(method.DeclaringType))
            {
                switch (method.Name)
                {
                    case "ToString":
                        mappedMethodName = "toString";
                        break;
                }
            }

            if (mappedMethodName == null)
            {
                throw new NotSupportedException($"The method '{method}' is not supported.");
            }

            var ownerExpressionString = ExpressionProcessorPipeline.GetScriptFromConditionExpression(methodCallExpression.Object);

            // TODO: Add support for parameters
            result = $"{ownerExpressionString}.{mappedMethodName}()";

            return true;
        }

        private string ProcessStringMethodInvocation(MethodCallExpression methodCallExpression, MethodInfo methodInfo)
        {
            // Methods to implement:
            // contains, startswith, endswith, indexof, lastindexof, insert (!!! not called insert in JS - its splice or some shit like that), padleft, padright, remove (!!!), replace, split, substring,
            var argumentList = new List<string>();

            string mappedMethodName = null;

            if (methodInfo == String_ToLower)
            {
                mappedMethodName = "toLowerCase";
            }
            else if (methodInfo == String_ToUpper)
            {
                mappedMethodName = "toLowerCase";
            }
            else if (methodInfo == String_TrimStart)
            {
                mappedMethodName = "trimStart";
            }
            else if (methodInfo == String_TrimEnd)
            {
                mappedMethodName = "trimEnd";
            }
            else if (methodInfo == String_Trim)
            {
                mappedMethodName = "trim";
            }
            else if (methodInfo == String_Substring_StartIndex)
            {
                mappedMethodName = "substring";

                var startIndexExpression = methodCallExpression.Arguments[0];
                var startIndex = ExpressionProcessorPipeline.GetScriptFromConditionExpression(startIndexExpression);

                argumentList.Add(startIndex);
            }
            else if (methodInfo == String_Substring_StartIndex_Length)
            {
                mappedMethodName = "substring";

                var startIndexExpression = methodCallExpression.Arguments[0];
                var lengthExpression = methodCallExpression.Arguments[1];

                var startIndex = ExpressionProcessorPipeline.GetScriptFromConditionExpression(startIndexExpression);
                var length = ExpressionProcessorPipeline.GetScriptFromConditionExpression(lengthExpression);

                argumentList.Add(startIndex);
                argumentList.Add(length);
            }
            else
            {
                throw new NotSupportedException($"The method '{methodInfo}' is not supported.");
            }

            var ownerExpressionString = ExpressionProcessorPipeline.GetScriptFromConditionExpression(methodCallExpression.Object);

            var arguments = string.Join(", ", argumentList);
            return $"{ownerExpressionString}.{mappedMethodName}({arguments})";
        }
    }
}