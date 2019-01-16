using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions
{
    public static class RuntimeExpressionValueResolver
    {
        public static object GetValue(Expression expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            switch (expression)
            {
                case ConstantExpression constantExpression:
                    return GetValue(constantExpression);

                case MemberExpression memberExpression:
                    return GetValue(memberExpression);

                case MethodCallExpression methodCallExpression:
                    return GetValue(methodCallExpression);

                default:
                    throw new NotSupportedException($"The expression of type '{expression.GetType().FullName}' is not supported.");
            }
        }

        public static object GetValue(ConstantExpression constantExpression)
        {
            if (constantExpression == null)
            {
                throw new ArgumentNullException(nameof(constantExpression));
            }

            return constantExpression.Value;
        }

        public static object GetValue(MethodCallExpression methodCallExpression)
        {
            if (methodCallExpression == null)
            {
                throw new ArgumentNullException(nameof(methodCallExpression));
            }

            var method = methodCallExpression.Method;
            var methodTarget = GetValue(methodCallExpression.Object);
            var arguments = methodCallExpression.Arguments.Select(expr => GetValue(expr)).ToArray();

            return method.Invoke(methodTarget, arguments);
        }

        public static object GetValue(MemberExpression memberExpression)
        {
            if (memberExpression == null)
            {
                throw new ArgumentNullException(nameof(memberExpression));
            }

            var fieldInfo = memberExpression.Member as FieldInfo;
            var propertyInfo = memberExpression.Member as PropertyInfo;

            if (fieldInfo != null)
            {
                var target = fieldInfo.IsStatic ? null : GetValue(memberExpression.Expression);

                return fieldInfo.GetValue(target);
            }

            if (propertyInfo != null)
            {
                var target = propertyInfo.GetMethod.IsStatic ? null : GetValue(memberExpression.Expression);

                return propertyInfo.GetValue(target);
            }

            throw new NotSupportedException($"Only field and property members are supported.");
        }
    }
}