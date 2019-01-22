using System;
using System.Linq.Expressions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.Abstractions;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Utilitites
{
    public class PatchScriptBuilder : IPatchScriptBuilder
    {
        private const string DocumentParameterName = "this";

        private readonly IExpressionProcessorPipeline _expressionProcessorPipeline;
        private readonly IPatchScriptConditionBuilder _patchScriptConditionBuilder;
        private readonly IPatchScriptBodyBuilder _patchScriptBodyBuilder;

        public PatchScriptBuilder(IExpressionProcessorPipeline expressionProcessorPipeline, IPatchScriptConditionBuilder patchScriptConditionBuilder, IPatchScriptBodyBuilder patchScriptBodyBuilder)
        {
            _expressionProcessorPipeline = expressionProcessorPipeline ?? throw new ArgumentNullException(nameof(expressionProcessorPipeline));
            _patchScriptConditionBuilder = patchScriptConditionBuilder ?? throw new ArgumentNullException(nameof(patchScriptConditionBuilder));
            _patchScriptBodyBuilder = patchScriptBodyBuilder ?? throw new ArgumentNullException(nameof(patchScriptBodyBuilder));
        }

        public string CreateConditionalPatchScript<TDocument>(PropertyUpdateDescriptor[] propertyUpdates, Expression<Func<TDocument, bool>> condition, ScriptParameterDictionary parameters)
        {
            var scriptCondition = _patchScriptConditionBuilder.CreateScriptCondition(condition, parameters);
            var scriptBody = _patchScriptBodyBuilder.CreateScriptBody(propertyUpdates, parameters);

            return string.Join(
                "\n",
                $"if ({scriptCondition}) {{",
                scriptBody,
                "}");
        }
    }
}