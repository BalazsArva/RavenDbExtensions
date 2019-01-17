using System;
using System.Linq.Expressions;
using System.Reflection;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions
{
    public static class ExpressionHelper
    {
        /// <summary>
        /// Determines whether the provided expression acts upon a runtime object. Acting upon a runtime object means
        /// an arbitrarily complex chain of field or property accesses and calling methods on a runtime or in-memory object rather than a parameter.
        /// </summary>
        /// <param name="expression">
        /// The expression about which it should be determined whether it is runtime object-bound.
        /// </param>
        /// <returns>
        /// True if the provided expression is runtime object-bound and false if it is parameter-bound.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// Thrown when a not supported expression type is encountered when processing the root expression.
        /// Only <see cref="LambdaExpression"/>, <see cref="MemberExpression"/>, <see cref="ParameterExpression"/>,
        /// <see cref="MethodCallExpression"/> and <see cref="ConstantExpression"/> types might be present in the root expression.
        /// </exception>
        public static bool IsRuntimeObjectBoundExpression(Expression expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            if (expression is ConstantExpression constantExpression)
            {
                return true;
            }
            else if (expression is ParameterExpression)
            {
                return false;
            }
            else if (expression is LambdaExpression lambdaExpression)
            {
                return IsRuntimeObjectBoundExpression(lambdaExpression.Body);
            }
            else if (expression is MemberExpression memberExpression)
            {
                // If this is a static field or property, then it is a runtime object bound expression.
                if (memberExpression.Member is PropertyInfo propertyInfo && propertyInfo.GetMethod.IsStatic ||
                    memberExpression.Member is FieldInfo fieldInfo && fieldInfo.IsStatic)
                {
                    return true;
                }

                return IsRuntimeObjectBoundExpression(memberExpression.Expression);
            }
            else if (expression is MethodCallExpression methodCallExpression)
            {
                return IsRuntimeObjectBoundExpression(methodCallExpression.Object);
            }
            else if (expression is BinaryExpression binaryExpression)
            {
                return
                    IsRuntimeObjectBoundExpression(binaryExpression.Left) &&
                    IsRuntimeObjectBoundExpression(binaryExpression.Right);
            }
            else if (expression is UnaryExpression unaryExpression)
            {
                return IsRuntimeObjectBoundExpression(unaryExpression.Operand);
            }
            else if (expression is ConditionalExpression conditionalExpression)
            {
                return
                    IsRuntimeObjectBoundExpression(conditionalExpression.Test) &&
                    IsRuntimeObjectBoundExpression(conditionalExpression.IfTrue) &&
                    IsRuntimeObjectBoundExpression(conditionalExpression.IfFalse);
            }

            throw new NotSupportedException($"Expression of type '{expression.GetType().FullName}' is not supported.");
        }

        /// <summary>
        /// Determines whether the provided expression acts upon an expression parameter. Acting upon a parameter means
        /// an arbitrarily complex chain of field or property accesses and calling methods on them.
        /// </summary>
        /// <param name="expression">
        /// The expression about which it should be determined whether it is parameter-bound.
        /// </param>
        /// <returns>
        /// True if the provided expression is parameter-bound and false if it is runtime object-bound.
        /// </returns>
        /// <exception cref="NotSupportedException">
        /// Thrown when a not supported expression type is encountered when processing the root expression.
        /// Only <see cref="LambdaExpression"/>, <see cref="MemberExpression"/>, <see cref="ParameterExpression"/>,
        /// <see cref="MethodCallExpression"/> and <see cref="ConstantExpression"/> types might be present in the root expression.
        /// </exception>
        public static bool IsParameterBoundExpression(Expression expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            if (expression is ParameterExpression)
            {
                return true;
            }
            else if (expression is LambdaExpression lambdaExpression)
            {
                return IsParameterBoundExpression(lambdaExpression.Body);
            }
            else if (expression is MemberExpression memberExpression)
            {
                // If this is a static field or property, then it is a runtime object bound expression.
                // TODO: What if we do something like "p => String.IsNullOrEmpty(p.SomeStringProperty)"? If the static method call can be translated, we should allow that.
                if (memberExpression.Member is PropertyInfo propertyInfo && propertyInfo.GetMethod.IsStatic ||
                    memberExpression.Member is FieldInfo fieldInfo && fieldInfo.IsStatic)
                {
                    return false;
                }

                return IsParameterBoundExpression(memberExpression.Expression);
            }
            else if (expression is MethodCallExpression methodCallExpression)
            {
                return IsParameterBoundExpression(methodCallExpression.Object);
            }
            else if (expression is ConstantExpression constantExpression)
            {
                return false;
            }
            else if (expression is BinaryExpression binaryExpression)
            {
                return
                    IsParameterBoundExpression(binaryExpression.Left) &&
                    IsParameterBoundExpression(binaryExpression.Right);
            }
            else if (expression is UnaryExpression unaryExpression)
            {
                return IsParameterBoundExpression(unaryExpression.Operand);
            }
            else if (expression is ConditionalExpression conditionalExpression)
            {
                return
                    IsParameterBoundExpression(conditionalExpression.Test) &&
                    IsParameterBoundExpression(conditionalExpression.IfTrue) &&
                    IsParameterBoundExpression(conditionalExpression.IfFalse);
            }

            throw new NotSupportedException($"Expression of type '{expression.GetType().FullName}' is not supported.");
        }
    }
}