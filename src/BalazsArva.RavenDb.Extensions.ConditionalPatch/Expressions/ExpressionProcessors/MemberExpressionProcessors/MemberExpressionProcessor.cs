using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.Abstractions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Utilitites;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.ExpressionProcessors.MemberExpressionProcessors
{
    public class MemberExpressionProcessor : IExpressionProcessor
    {
        private readonly IEnumerable<IExpressionProcessor<MemberExpression>> expressionProcessors;

        // TODO: Handle properties which require special treatment, such as:
        // - DateTime(Offset).(Year|Month|Day|Hour|Minute|Second|Millisecond|etc.)
        public MemberExpressionProcessor()
        {
            // TODO: Consider LINQ extension methods as well!
            expressionProcessors = new List<IExpressionProcessor<MemberExpression>>
            {
                new StringMemberExpressionProcessor(),
                new NullableMemberExpressionProcessor(),

                // This must come before CollectionCountMemberExpressionProcessor
                new DictionaryMemberExpressionProcessor(),

                new CollectionCountMemberExpressionProcessor(),

                // This must be the last one, otherwise this will wrongly return the specially treated ones as well!
                new HandleAllMemberExpressionProcessor()
            };
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

            if (expression.NodeType != ExpressionType.MemberAccess)
            {
                result = default;
                return false;
            }

            foreach (var processor in expressionProcessors)
            {
                if (processor.TryProcess((MemberExpression)expression, parameters, out result))
                {
                    return true;
                }
            }

            result = default;
            return false;
        }
    }
}