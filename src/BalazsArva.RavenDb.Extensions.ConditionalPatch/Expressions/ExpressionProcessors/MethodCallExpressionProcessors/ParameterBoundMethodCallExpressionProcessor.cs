using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.Abstractions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Utilitites;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.ExpressionProcessors.MethodCallExpressionProcessors
{
    public class ParameterBoundMethodCallExpressionProcessor : IExpressionProcessor
    {
        private static readonly Type ObjectType = typeof(object);
        private static readonly Type StringType = typeof(string);

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
                // TODO: Do this more elegantly
                var tmp = new StringMethodCallExpressionProcessor();

                return tmp.TryProcess(methodCallExpression, parameters, out result);
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

            var ownerExpressionString = ExpressionParser.CreateJsScriptFromExpression(methodCallExpression.Object, parameters);

            // TODO: Add support for parameters
            result = $"{ownerExpressionString}.{mappedMethodName}()";

            return true;
        }
    }
}