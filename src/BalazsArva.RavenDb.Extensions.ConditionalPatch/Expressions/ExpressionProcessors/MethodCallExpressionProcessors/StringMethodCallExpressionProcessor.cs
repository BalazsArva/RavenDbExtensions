using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.Abstractions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Utilitites;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.ExpressionProcessors.MethodCallExpressionProcessors
{
    public class StringMethodCallExpressionProcessor : IExpressionProcessor<MethodCallExpression>
    {
        private static readonly Type stringType = typeof(string);

        private static readonly MethodInfo NonStatic_Contains_Char = stringType.GetMethod("Contains", new[] { typeof(char) });
        private static readonly MethodInfo NonStatic_Contains_String = stringType.GetMethod("Contains", new[] { typeof(string) });

        private static readonly MethodInfo NonStatic_ToLower = stringType.GetMethod("ToLower", Array.Empty<Type>());
        private static readonly MethodInfo NonStatic_ToUpper = stringType.GetMethod("ToUpper", Array.Empty<Type>());

        private static readonly MethodInfo NonStatic_Trim = stringType.GetMethod("Trim", Array.Empty<Type>());
        private static readonly MethodInfo NonStatic_TrimStart = stringType.GetMethod("TrimStart", Array.Empty<Type>());
        private static readonly MethodInfo NonStatic_TrimEnd = stringType.GetMethod("TrimEnd", Array.Empty<Type>());

        private static readonly MethodInfo NonStatic_Substring_StartIndex = stringType.GetMethod("Substring", new[] { typeof(int) });
        private static readonly MethodInfo NonStatic_Substring_StartIndex_Length = stringType.GetMethod("Substring", new[] { typeof(int), typeof(int) });

        private static readonly MethodInfo Static_IsNullOrEmpty = stringType.GetMethod("IsNullOrEmpty");
        private static readonly MethodInfo Static_IsNullOrWhiteSpace = stringType.GetMethod("IsNullOrWhiteSpace");
        //private static readonly MethodInfo Static_Concat = stringType.GetMethod("Concat");
        //private static readonly MethodInfo Static_Join = stringType.GetMethod("Join");

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

            if (methodCallExpression.Method.DeclaringType != stringType)
            {
                result = default;

                return false;
            }

            return methodCallExpression.Object == null
                ? TryProcessStaticStringMethodInvocation(methodCallExpression, methodCallExpression.Method, parameters, out result)
                : TryProcessNonStaticStringMethodInvocation(methodCallExpression, methodCallExpression.Method, parameters, out result);
        }

        private bool TryProcessStaticStringMethodInvocation(MethodCallExpression methodCallExpression, MethodInfo methodInfo, ScriptParameterDictionary parameters, out string result)
        {
            if (methodInfo == Static_IsNullOrEmpty)
            {
                var methodParameterExpression = methodCallExpression.Arguments[0];
                var methodParameter = ExpressionParser.CreateJsScriptFromExpression(methodParameterExpression, parameters);

                result = $"({methodParameter} == null || {methodParameter} == '')";
                return true;
            }
            else if (methodInfo == Static_IsNullOrWhiteSpace)
            {
                var methodParameterExpression = methodCallExpression.Arguments[0];
                var methodParameter = ExpressionParser.CreateJsScriptFromExpression(methodParameterExpression, parameters);

                result = $"({methodParameter} == null || {methodParameter}.trim() == '')";
                return true;
            }
            /*
            else if (methodInfo == Static_Concat)
            {
            }
            else if (methodInfo == Static_Join)
            {
            }
            */
            else
            {
                result = null;
                return false;
            }
        }

        private bool TryProcessNonStaticStringMethodInvocation(MethodCallExpression methodCallExpression, MethodInfo methodInfo, ScriptParameterDictionary parameters, out string result)
        {
            // Methods to implement:
            // startswith, endswith, indexof, lastindexof, insert (!!! not called insert in JS - its splice or some shit like that), padleft, padright, remove (!!!), replace, split, substring,
            var argumentList = new List<string>();

            string mappedMethodName = null;

            if (methodInfo == NonStatic_Contains_Char || methodInfo == NonStatic_Contains_String)
            {
                mappedMethodName = "toLowerCase";

                var searchInValue = ExpressionParser.CreateJsScriptFromExpression(methodCallExpression.Object, parameters);

                var searchForValueExpression = methodCallExpression.Arguments[0];
                var searchForValue = ExpressionParser.CreateJsScriptFromExpression(searchForValueExpression, parameters);

                result = $"({searchInValue}.indexOf({searchForValue}) != -1)";
                return true;
            }
            else if (methodInfo == NonStatic_ToLower)
            {
                mappedMethodName = "toLowerCase";
            }
            else if (methodInfo == NonStatic_ToUpper)
            {
                mappedMethodName = "toLowerCase";
            }
            else if (methodInfo == NonStatic_TrimStart)
            {
                mappedMethodName = "trimStart";
            }
            else if (methodInfo == NonStatic_TrimEnd)
            {
                mappedMethodName = "trimEnd";
            }
            else if (methodInfo == NonStatic_Trim)
            {
                mappedMethodName = "trim";
            }
            else if (methodInfo == NonStatic_Substring_StartIndex)
            {
                mappedMethodName = "substring";

                var startIndexExpression = methodCallExpression.Arguments[0];
                var startIndex = ExpressionParser.CreateJsScriptFromExpression(startIndexExpression, parameters);

                argumentList.Add(startIndex);
            }
            else if (methodInfo == NonStatic_Substring_StartIndex_Length)
            {
                mappedMethodName = "substring";

                var startIndexExpression = methodCallExpression.Arguments[0];
                var lengthExpression = methodCallExpression.Arguments[1];

                var startIndex = ExpressionParser.CreateJsScriptFromExpression(startIndexExpression, parameters);
                var length = ExpressionParser.CreateJsScriptFromExpression(lengthExpression, parameters);

                argumentList.Add(startIndex);
                argumentList.Add(length);
            }
            else
            {
                result = default;
                return false;
            }

            var ownerExpressionString = ExpressionParser.CreateJsScriptFromExpression(methodCallExpression.Object, parameters);

            var arguments = string.Join(", ", argumentList);
            result = $"{ownerExpressionString}.{mappedMethodName}({arguments})";

            return true;
        }
    }
}