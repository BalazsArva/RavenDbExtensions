using System;
using System.Linq.Expressions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.Abstractions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Utilitites;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.ExpressionProcessors
{
    public class BinaryExpressionProcessor : IExpressionProcessor
    {
        public bool TryProcess(Expression expression, ScriptParameterDictionary parameters, out string result)
        {
            var binaryExpression = expression as BinaryExpression;
            if (binaryExpression == null)
            {
                result = default;

                return false;
            }

            var leftExpression = TrySimplifyExpression(binaryExpression.Left);
            var rightExpression = TrySimplifyExpression(binaryExpression.Right);

            var operation = binaryExpression.NodeType;
            var left = ExpressionParser.CreateJsScriptFromExpression(leftExpression, parameters);
            var right = ExpressionParser.CreateJsScriptFromExpression(rightExpression, parameters);

            switch (operation)
            {
                case ExpressionType.Add:
                    result = $"({left} + {right})";
                    return true;

                case ExpressionType.And:
                    result = $"({left} & {right})";
                    return true;

                case ExpressionType.AndAlso:
                    result = $"({left} && {right})";
                    return true;

                case ExpressionType.ArrayIndex:
                    result = $"{left}[{right}]";
                    return true;

                case ExpressionType.Coalesce:
                    result = $"({left} != null ? {left} : {right})";
                    return true;

                case ExpressionType.Divide:
                    result = $"({left} / {right})";
                    return true;

                case ExpressionType.Equal:
                    result = $"({left} == {right})";
                    return true;

                case ExpressionType.ExclusiveOr:
                    result = $"({left} ^ {right})";
                    return true;

                case ExpressionType.GreaterThan:
                    result = $"({left} > {right})";
                    return true;

                case ExpressionType.GreaterThanOrEqual:
                    result = $"({left} >= {right})";
                    return true;

                case ExpressionType.LessThan:
                    result = $"({left} < {right})";
                    return true;

                case ExpressionType.LessThanOrEqual:
                    result = $"({left} <= {right})";
                    return true;

                case ExpressionType.Modulo:
                    result = $"({left} % {right})";
                    return true;

                case ExpressionType.Multiply:
                    result = $"({left} * {right})";
                    return true;

                case ExpressionType.NotEqual:
                    result = $"({left} != {right})";
                    return true;

                case ExpressionType.Or:
                    result = $"({left} | {right})";
                    return true;

                case ExpressionType.OrElse:
                    result = $"({left} || {right})";
                    return true;

                case ExpressionType.Subtract:
                    result = $"({left} - {right})";
                    return true;

                case ExpressionType.Assign:
                    result = $"({left} = {right})";
                    return true;

                case ExpressionType.Index:
                    result = $"{left}[{right}]";
                    return true;

                case ExpressionType.AddAssign:
                    result = $"({left} += {right})";
                    return true;

                case ExpressionType.AndAssign:
                    result = $"({left} &= {right})";
                    return true;

                case ExpressionType.DivideAssign:
                    result = $"({left} /= {right})";
                    return true;

                case ExpressionType.ExclusiveOrAssign:
                    result = $"({left} ^= {right})";
                    return true;

                case ExpressionType.ModuloAssign:
                    result = $"({left} %= {right})";
                    return true;

                case ExpressionType.MultiplyAssign:
                    result = $"({left} *= {right})";
                    return true;

                case ExpressionType.OrAssign:
                    result = $"({left} |= {right})";
                    return true;

                case ExpressionType.SubtractAssign:
                    result = $"({left} -= {right})";
                    return true;

                default:
                    throw new NotSupportedException($"Binary expression with '{operation.ToString()}' operator is not supported.");
            }
        }

        private Expression TrySimplifyExpression(Expression expression)
        {
            if (ExpressionHelper.IsRuntimeObjectBoundExpression(expression))
            {
                var convertExpression = Expression.Convert(expression, typeof(object));
                var lambdaExpression = Expression.Lambda<Func<object>>(expression);

                var compiledLambdaExpression = lambdaExpression.Compile();
                var value = compiledLambdaExpression();

                return Expression.Constant(value);
            }

            return expression;
        }
    }
}