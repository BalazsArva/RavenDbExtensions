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
        private const char DefaultPaddingChar = ' ';

        private static readonly Type StringType = typeof(string);
        private static readonly Type IntType = typeof(int);
        private static readonly Type CharType = typeof(char);

        private static readonly MethodInfo NonStatic_Contains_Char = StringType.GetMethod("Contains", new[] { CharType });
        private static readonly MethodInfo NonStatic_Contains_String = StringType.GetMethod("Contains", new[] { StringType });

        private static readonly MethodInfo NonStatic_Insert = StringType.GetMethod("Insert", new[] { IntType, StringType });

        private static readonly MethodInfo NonStatic_Remove_StartIndex = StringType.GetMethod("Remove", new[] { IntType });
        private static readonly MethodInfo NonStatic_Remove_StartIndex_Count = StringType.GetMethod("Remove", new[] { IntType, IntType });

        private static readonly MethodInfo NonStatic_PadLeft_TotalWith = StringType.GetMethod("PadLeft", new[] { IntType });
        private static readonly MethodInfo NonStatic_PadLeft_TotalWith_PaddingChar = StringType.GetMethod("PadLeft", new[] { IntType, CharType });

        private static readonly MethodInfo NonStatic_PadRight_TotalWith = StringType.GetMethod("PadRight", new[] { IntType });
        private static readonly MethodInfo NonStatic_PadRight_TotalWith_PaddingChar = StringType.GetMethod("PadRight", new[] { IntType, CharType });

        private static readonly MethodInfo NonStatic_ToLower = StringType.GetMethod("ToLower", Array.Empty<Type>());
        private static readonly MethodInfo NonStatic_ToUpper = StringType.GetMethod("ToUpper", Array.Empty<Type>());

        private static readonly MethodInfo NonStatic_Trim = StringType.GetMethod("Trim", Array.Empty<Type>());
        private static readonly MethodInfo NonStatic_TrimStart = StringType.GetMethod("TrimStart", Array.Empty<Type>());
        private static readonly MethodInfo NonStatic_TrimEnd = StringType.GetMethod("TrimEnd", Array.Empty<Type>());

        private static readonly MethodInfo NonStatic_StartsWith_Value = StringType.GetMethod("StartsWith", new[] { StringType });
        private static readonly MethodInfo NonStatic_EndsWith_Value = StringType.GetMethod("EndsWith", new[] { StringType });

        private static readonly MethodInfo NonStatic_Substring_StartIndex = StringType.GetMethod("Substring", new[] { IntType });
        private static readonly MethodInfo NonStatic_Substring_StartIndex_Length = StringType.GetMethod("Substring", new[] { IntType, IntType });

        private static readonly MethodInfo Static_IsNullOrEmpty = StringType.GetMethod("IsNullOrEmpty");
        private static readonly MethodInfo Static_IsNullOrWhiteSpace = StringType.GetMethod("IsNullOrWhiteSpace");
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

            if (methodCallExpression.Method.DeclaringType != StringType)
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
            // indexof, lastindexof, replace (!must do with "/chars/g", but must escape regex control characters!), split, equals, compareTo
            var argumentList = new List<string>();

            string mappedMethodName = null;

            if (methodInfo == NonStatic_Contains_Char || methodInfo == NonStatic_Contains_String)
            {
                var searchInValue = ExpressionParser.CreateJsScriptFromExpression(methodCallExpression.Object, parameters);

                var searchForValueExpression = methodCallExpression.Arguments[0];
                var searchForValue = ExpressionParser.CreateJsScriptFromExpression(searchForValueExpression, parameters);

                result = $"({searchInValue}.indexOf({searchForValue}) != -1)";
                return true;
            }
            else if (methodInfo == NonStatic_Insert)
            {
                var insertIntoValue = ExpressionParser.CreateJsScriptFromExpression(methodCallExpression.Object, parameters);

                var indexValueExpression = methodCallExpression.Arguments[0];
                var indexValue = ExpressionParser.CreateJsScriptFromExpression(indexValueExpression, parameters);

                var insertValueExpression = methodCallExpression.Arguments[1];
                var insertValue = ExpressionParser.CreateJsScriptFromExpression(insertValueExpression, parameters);

                result = $"({insertIntoValue}.substring(0, {indexValue}) + {insertValue} + {insertIntoValue}.substring({indexValue}))";
                return true;
            }
            else if (methodInfo == NonStatic_Remove_StartIndex)
            {
                mappedMethodName = "substring";

                var startIndexValueExpression = methodCallExpression.Arguments[0];
                var startIndexValue = ExpressionParser.CreateJsScriptFromExpression(startIndexValueExpression, parameters);

                argumentList.Add("0");
                argumentList.Add(startIndexValue);
            }
            else if (methodInfo == NonStatic_Remove_StartIndex_Count)
            {
                var removeFromValue = ExpressionParser.CreateJsScriptFromExpression(methodCallExpression.Object, parameters);

                var firstSegmentStartIndexExpression = methodCallExpression.Arguments[0];
                var firstSegmentStartIndexValue = ExpressionParser.CreateJsScriptFromExpression(firstSegmentStartIndexExpression, parameters);

                var countExpression = methodCallExpression.Arguments[1];

                var secondSegmentStartIndexExpression = Expression.Add(firstSegmentStartIndexExpression, countExpression);
                var secondSegmentStartIndexValue = ExpressionParser.CreateJsScriptFromExpression(secondSegmentStartIndexExpression, parameters);

                result = $"({removeFromValue}.substring(0, {firstSegmentStartIndexValue}) + {removeFromValue}.substring({secondSegmentStartIndexValue}))";
                return true;
            }
            else if (methodInfo == NonStatic_StartsWith_Value)
            {
                mappedMethodName = "startsWith";

                var startWithValueExpression = methodCallExpression.Arguments[0];
                var startWithValue = ExpressionParser.CreateJsScriptFromExpression(startWithValueExpression, parameters);

                argumentList.Add(startWithValue);
            }
            else if (methodInfo == NonStatic_EndsWith_Value)
            {
                mappedMethodName = "endsWith";

                var startWithValueExpression = methodCallExpression.Arguments[0];
                var startWithValue = ExpressionParser.CreateJsScriptFromExpression(startWithValueExpression, parameters);

                argumentList.Add(startWithValue);
            }
            else if (methodInfo == NonStatic_PadLeft_TotalWith || methodInfo == NonStatic_PadLeft_TotalWith_PaddingChar)
            {
                mappedMethodName = "padStart";

                var totalWidthExpression = methodCallExpression.Arguments[0];
                var totalWidth = ExpressionParser.CreateJsScriptFromExpression(totalWidthExpression, parameters);

                Expression paddingCharExpression = Expression.Constant(DefaultPaddingChar);
                if (methodInfo == NonStatic_PadLeft_TotalWith_PaddingChar)
                {
                    paddingCharExpression = methodCallExpression.Arguments[1];
                }

                var paddingChar = ExpressionParser.CreateJsScriptFromExpression(paddingCharExpression, parameters);

                argumentList.Add(totalWidth);
                argumentList.Add(paddingChar);
            }
            else if (methodInfo == NonStatic_PadRight_TotalWith || methodInfo == NonStatic_PadRight_TotalWith_PaddingChar)
            {
                mappedMethodName = "padEnd";

                var totalWidthExpression = methodCallExpression.Arguments[0];
                var totalWidth = ExpressionParser.CreateJsScriptFromExpression(totalWidthExpression, parameters);

                Expression paddingCharExpression = Expression.Constant(DefaultPaddingChar);
                if (methodInfo == NonStatic_PadRight_TotalWith_PaddingChar)
                {
                    paddingCharExpression = methodCallExpression.Arguments[1];
                }

                var paddingChar = ExpressionParser.CreateJsScriptFromExpression(paddingCharExpression, parameters);

                argumentList.Add(totalWidth);
                argumentList.Add(paddingChar);
            }
            else if (methodInfo == NonStatic_ToLower)
            {
                mappedMethodName = "toLowerCase";
            }
            else if (methodInfo == NonStatic_ToUpper)
            {
                mappedMethodName = "toUpperCase";
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