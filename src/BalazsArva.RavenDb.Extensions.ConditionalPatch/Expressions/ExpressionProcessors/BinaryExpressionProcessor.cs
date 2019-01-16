using System;
using System.Linq.Expressions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.Abstractions;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.ExpressionProcessors
{
    public class BinaryExpressionProcessor : IExpressionProcessor
    {
        public bool TryProcess(Expression expression, out string result)
        {
            var binaryExpression = expression as BinaryExpression;

            if (binaryExpression == null)
            {
                result = default;

                return false;
            }

            var operation = binaryExpression.NodeType;
            var left = ExpressionProcessorPipeline.GetScriptFromConditionExpression(binaryExpression.Left);
            var right = ExpressionProcessorPipeline.GetScriptFromConditionExpression(binaryExpression.Right);

            // TODO: Implement the remaining ones
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

                case ExpressionType.ArrayLength:
                case ExpressionType.ArrayIndex:
                //case ExpressionType.Call:
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

                //case ExpressionType.Invoke:

                case ExpressionType.LessThan:
                    result = $"({left} < {right})";
                    return true;

                case ExpressionType.LessThanOrEqual:
                    result = $"({left} <= {right})";
                    return true;

                //case ExpressionType.MemberAccess:

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

                //case ExpressionType.Index:
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
    }
}