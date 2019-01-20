using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.Abstractions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Utilitites;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.ExpressionProcessors.MemberExpressionProcessors
{
    public class CountMemberExpressionProcessor : IExpressionProcessor<MemberExpression>
    {
        // Every collection type which resides in the System.Collections or System.Collections.Generic namespaces and
        // which has a Count property directly or indirectly implements either of these interfaces. These all must be
        // listed because these do not implement one another.
        private static readonly HashSet<Type> KnownTypes = new HashSet<Type>
        {
            typeof(ICollection),
            typeof(ICollection<>),
            typeof(IReadOnlyCollection<>)
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
            if (memberExpression.Member is PropertyInfo propertyInfo && IsSpeciallyTreatedCountProperty(propertyInfo))
            {
                var ownerExpressionString = ExpressionParser.CreateJsScriptFromExpression(memberExpression.Expression, parameters);

                result = $"{ownerExpressionString}.length";
                return true;
            }

            result = default;
            return false;
        }

        private bool IsSpeciallyTreatedCountProperty(PropertyInfo propertyInfo)
        {
            if (propertyInfo.Name != "Count")
            {
                return false;
            }

            return propertyInfo.DeclaringType
                .GetInterfaces()
                .Select(i => i.IsGenericType ? i.GetGenericTypeDefinition() : i)
                .Any(i => KnownTypes.Contains(i));
        }
    }
}