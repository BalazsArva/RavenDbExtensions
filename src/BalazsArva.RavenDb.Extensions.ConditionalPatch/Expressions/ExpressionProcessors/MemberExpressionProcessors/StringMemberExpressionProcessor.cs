using System;
using System.Linq.Expressions;
using System.Reflection;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.Abstractions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Utilitites;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.ExpressionProcessors.MemberExpressionProcessors
{
    public class StringMemberExpressionProcessor : IExpressionProcessor<MemberExpression>
    {
        private static readonly Type StringType = typeof(string);

        private static readonly PropertyInfo NonStatic_Length = StringType.GetProperty("Length");

        private readonly IExpressionProcessorPipeline _expressionProcessorPipeline;

        public StringMemberExpressionProcessor(IExpressionProcessorPipeline expressionProcessorPipeline)
        {
            _expressionProcessorPipeline = expressionProcessorPipeline ?? throw new ArgumentNullException(nameof(expressionProcessorPipeline));
        }

        public bool TryProcess(MemberExpression memberExpression, ScriptParameterDictionary parameters, out string result)
        {
            if (memberExpression == null)
            {
                throw new ArgumentNullException(nameof(memberExpression));
            }

            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            if (memberExpression.Member is PropertyInfo propertyInfo && propertyInfo == NonStatic_Length)
            {
                var ownerExpressionString = _expressionProcessorPipeline.ProcessExpression(memberExpression.Expression, parameters);

                result = $"{ownerExpressionString}.length";
                return true;
            }

            result = default;
            return false;
        }
    }
}