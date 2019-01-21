using System;
using System.Linq.Expressions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.Abstractions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Utilitites;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.ExpressionProcessors
{
    public class UnaryExpressionProcessor : IExpressionProcessor
    {
        private readonly IExpressionProcessorPipeline _expressionProcessorPipeline;

        public UnaryExpressionProcessor(IExpressionProcessorPipeline expressionProcessorPipeline)
        {
            _expressionProcessorPipeline = expressionProcessorPipeline ?? throw new ArgumentNullException(nameof(expressionProcessorPipeline));
        }

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

            var unaryExpression = expression as UnaryExpression;
            if (unaryExpression == null)
            {
                result = default;

                return false;
            }

            var operation = unaryExpression.NodeType;
            var operand = _expressionProcessorPipeline.ProcessExpression(unaryExpression.Operand, parameters);

            switch (operation)
            {
                case ExpressionType.ArrayLength:
                    result = $"{operand}.length";
                    return true;

                case ExpressionType.Convert:
                    result = operand;
                    return true;

                case ExpressionType.Negate:
                    result = $"-({operand})";
                    return true;

                case ExpressionType.UnaryPlus:
                    result = $"+({operand})";
                    return true;

                case ExpressionType.Not:
                    result = $"!({operand})";
                    return true;

                case ExpressionType.Decrement:
                    result = $"({operand} - 1)";
                    return true;

                case ExpressionType.Increment:
                    result = $"({operand} + 1)";
                    return true;

                case ExpressionType.PreIncrementAssign:
                    result = $"(++{operand})";
                    return true;

                case ExpressionType.PreDecrementAssign:
                    result = $"(--{operand})";
                    return true;

                case ExpressionType.PostIncrementAssign:
                    result = $"({operand}++)";
                    return true;

                case ExpressionType.PostDecrementAssign:
                    result = $"({operand}--)";
                    return true;

                default:
                    result = default;
                    return false;
            }
        }
    }
}