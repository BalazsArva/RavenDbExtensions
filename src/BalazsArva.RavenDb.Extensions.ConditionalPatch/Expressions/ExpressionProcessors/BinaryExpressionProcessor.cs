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

            var operation = binaryExpression.NodeType;
            var left = ExpressionParser.CreateJsScriptFromExpression(binaryExpression.Left, parameters);
            var right = ExpressionParser.CreateJsScriptFromExpression(binaryExpression.Right, parameters);

            switch (operation)
            {
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
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
                case ExpressionType.MultiplyChecked:
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
                case ExpressionType.SubtractChecked:
                    result = $"({left} - {right})";
                    return true;

                case ExpressionType.Assign:
                    result = $"({left} = {right})";
                    return true;

                case ExpressionType.Index:
                    result = $"{left}[{right}]";
                    return true;

                case ExpressionType.AddAssign:
                case ExpressionType.AddAssignChecked:
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
                case ExpressionType.MultiplyAssignChecked:
                    result = $"({left} *= {right})";
                    return true;

                case ExpressionType.OrAssign:
                    result = $"({left} |= {right})";
                    return true;

                case ExpressionType.SubtractAssign:
                case ExpressionType.SubtractAssignChecked:
                    result = $"({left} -= {right})";
                    return true;

                default:
                    throw new NotSupportedException($"Binary expression with '{operation.ToString()}' operator is not supported.");
            }
        }
    }
}