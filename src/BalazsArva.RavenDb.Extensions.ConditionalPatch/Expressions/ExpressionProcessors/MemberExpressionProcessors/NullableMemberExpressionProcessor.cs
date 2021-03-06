﻿using System;
using System.Linq.Expressions;
using System.Reflection;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.Abstractions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Utilitites;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.ExpressionProcessors.MemberExpressionProcessors
{
    public class NullableMemberExpressionProcessor : IExpressionProcessor<MemberExpression>
    {
        private static readonly Type NullableType = typeof(Nullable<>);

        private readonly IExpressionProcessorPipeline _expressionProcessorPipeline;

        public NullableMemberExpressionProcessor(IExpressionProcessorPipeline expressionProcessorPipeline)
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

            if (memberExpression.Member is PropertyInfo propertyInfo)
            {
                var memberOwnerType = propertyInfo.DeclaringType;
                if (memberOwnerType.IsGenericType)
                {
                    memberOwnerType = memberOwnerType.GetGenericTypeDefinition();
                }

                if (memberOwnerType == NullableType)
                {
                    if (propertyInfo.Name == "HasValue")
                    {
                        var ownerExpressionString = _expressionProcessorPipeline.ProcessExpression(memberExpression.Expression, parameters);

                        result = $"({ownerExpressionString} != null)";
                        return true;
                    }

                    if (propertyInfo.Name == "Value")
                    {
                        var ownerExpressionString = _expressionProcessorPipeline.ProcessExpression(memberExpression.Expression, parameters);

                        result = $"{ownerExpressionString}";
                        return true;
                    }
                }
            }

            result = default;
            return false;
        }
    }
}