using System;
using System.Linq.Expressions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.Abstractions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Utilitites;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.ExpressionProcessors.MemberExpressionProcessors
{
    public class HandleAllMemberExpressionProcessor : IExpressionProcessor<MemberExpression>
    {
        private readonly IExpressionProcessorPipeline _expressionProcessorPipeline;

        public HandleAllMemberExpressionProcessor(IExpressionProcessorPipeline expressionProcessorPipeline)
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

            var ownerExpressionString = _expressionProcessorPipeline.ProcessExpression(memberExpression.Expression, parameters);

            // TODO: Consider cases when the property is called differently in the document than in the object model. Check out
            // Raven.Client.Documents.Linq.LinqPathProvider, maybe that can solve it out-of-the-box.
            result = $"{ownerExpressionString}.{memberExpression.Member.Name}";
            return true;
        }
    }
}