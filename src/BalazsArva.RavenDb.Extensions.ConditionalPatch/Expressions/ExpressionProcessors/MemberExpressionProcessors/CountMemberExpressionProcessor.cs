using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.Abstractions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Utilitites;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.ExpressionProcessors.MemberExpressionProcessors
{
    public class CountMemberExpressionProcessor : IExpressionProcessor<MemberExpression>
    {
        // TODO: Consider if there is anything else (ReadOnly variants?)
        private static readonly HashSet<Type> KnownTypes = new HashSet<Type>
        {
            typeof(ICollection),
            typeof(List<>),
            typeof(HashSet<>),
            typeof(Dictionary<,>)
        };

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

            // TODO: Tests
            // TODO: For Dictionary<TKey,TValue>, need to handle Keys and Values collections too!
            if (memberExpression.Member is PropertyInfo propertyInfo)
            {
                var memberOwnerType = propertyInfo.DeclaringType;
                if (memberOwnerType.IsGenericType)
                {
                    memberOwnerType = memberOwnerType.GetGenericTypeDefinition();
                }

                if (KnownTypes.Contains(memberOwnerType) && propertyInfo.Name == "Count")
                {
                    var ownerExpressionString = ExpressionParser.CreateJsScriptFromExpression(memberExpression.Expression, parameters);

                    result = $"{ownerExpressionString}.length";
                    return true;
                }
            }

            result = default;
            return false;
        }
    }
}