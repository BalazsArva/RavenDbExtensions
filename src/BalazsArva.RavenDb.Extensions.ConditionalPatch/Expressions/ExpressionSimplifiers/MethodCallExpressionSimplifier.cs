using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.Abstractions;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.ExpressionSimplifiers
{
    public class MethodCallExpressionSimplifier : IExpressionSimplifier
    {
        public bool TrySimplifyExpression(Expression expression, out Expression result)
        {
            if (expression is MethodCallExpression methodCallExpression)
            {
                var methodCallTarget = methodCallExpression.Object;
                var simplifiedArguments = methodCallExpression.Arguments.Select(arg => ExpressionSimplifier.SimplifyExpression(arg)).ToList();

                return methodCallTarget == null
                    ? TrySimplifyStaticMethodCall(methodCallExpression, simplifiedArguments, out result)
                    : TrySimplifyInstanceMethodCall(methodCallExpression, simplifiedArguments, out result);
            }

            result = default;

            return false;
        }

        private bool TrySimplifyStaticMethodCall(MethodCallExpression methodCallExpression, IEnumerable<Expression> simplifiedArguments, out Expression result)
        {
            // If all arguments are runtime-resolvable, we can invoke the method and return the result as a ConstantExpression.
            if (AllArgumentsAreRuntimeResolvable(simplifiedArguments))
            {
                var resolvedArguments = GetArgumentValues(simplifiedArguments);
                var resolvedValue = methodCallExpression.Method.Invoke(null, resolvedArguments);

                result = Expression.Constant(resolvedValue);

                return true;
            }

            // Otherwise, if any argument is parameter-bound, then evaluate whatever we can (i.e. there might be runtime-resolvable and non-runtime-resolvable
            // arguments) and update the method call expression with the simplified argument expressions. Static method calls can be valid in parameter-bound
            // expressions, such as doc => String.IsNullOrEmpty(doc.Title), etc.
            var updatedMethodCallExpression = methodCallExpression.Update(null, simplifiedArguments);
            if (updatedMethodCallExpression != methodCallExpression)
            {
                result = updatedMethodCallExpression;

                return true;
            }

            result = default;

            return false;
        }

        private bool TrySimplifyInstanceMethodCall(MethodCallExpression methodCallExpression, IEnumerable<Expression> simplifiedArguments, out Expression result)
        {
            var simplifiedMethodTargetExpression = ExpressionSimplifier.SimplifyExpression(methodCallExpression.Object);
            if (simplifiedMethodTargetExpression is ConstantExpression constantExpression)
            {
                if (AllArgumentsAreRuntimeResolvable(simplifiedArguments))
                {
                    // A method called on a runtime-resolved object, and all arguments are runtime-resolvable too.
                    // E.g.: "abc".PadLeft(10, 'a')
                    var resolvedArguments = GetArgumentValues(simplifiedArguments);
                    var resolvedValue = methodCallExpression.Method.Invoke(constantExpression.Value, resolvedArguments);

                    result = Expression.Constant(resolvedValue);

                    return true;
                }
                else
                {
                    // At least one of the arguments is parameter-bound. Update what we can.
                    // E.g.: doc => "abc".Equals(doc.Title)
                    var updatedMethodCallExpression = methodCallExpression.Update(constantExpression, simplifiedArguments);
                    if (updatedMethodCallExpression != methodCallExpression)
                    {
                        result = updatedMethodCallExpression;

                        return true;
                    }
                }
            }
            else
            {
                // Parameter-bound, resolve any arguments we can
                var updatedMethodCallExpression = methodCallExpression.Update(simplifiedMethodTargetExpression, simplifiedArguments);
                if (updatedMethodCallExpression != methodCallExpression)
                {
                    result = updatedMethodCallExpression;

                    return true;
                }
            }

            result = default;

            return false;
        }

        private bool AllArgumentsAreRuntimeResolvable(IEnumerable<Expression> arguments)
        {
            return arguments.All(arg => arg.NodeType == ExpressionType.Constant);
        }

        private object[] GetArgumentValues(IEnumerable<Expression> argumentExpressions)
        {
            return argumentExpressions.Select(arg => ((ConstantExpression)arg).Value).ToArray();
        }
    }
}