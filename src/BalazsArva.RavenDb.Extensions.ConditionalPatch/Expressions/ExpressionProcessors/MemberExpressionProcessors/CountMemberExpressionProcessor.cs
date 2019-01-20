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
            if (memberExpression.Member is PropertyInfo propertyInfo)
            {
                if (IsDictionaryKeysCollectionCountAccess(memberExpression, propertyInfo))
                {
                    // We need to navigate one level further up (i.e. doc.MyDictionary.Keys becomes doc.MyDictionary)
                    // because dictionaries are mapped to object literals with the respective keys when they become
                    // JSON, so the Keys segment becomes invalid.
                    var parentMemberExpression = ((MemberExpression)memberExpression.Expression).Expression;
                    var ownerExpressionString = ExpressionParser.CreateJsScriptFromExpression(parentMemberExpression, parameters);

                    // TODO: Check whether RavenDB provides a magic property for Dictionary key collections.
                    result = $"Object.keys({ownerExpressionString}).length";

                    return true;
                }

                if (IsDictionaryValuesCollectionCountAccess(memberExpression, propertyInfo))
                {
                    // We need to navigate one level further up (i.e. doc.MyDictionary.Values becomes doc.MyDictionary)
                    // because dictionaries are mapped to object literals with the respective keys when they become
                    // JSON, so the Values segment becomes invalid.
                    var parentMemberExpression = ((MemberExpression)memberExpression.Expression).Expression;
                    var ownerExpressionString = ExpressionParser.CreateJsScriptFromExpression(parentMemberExpression, parameters);

                    // TODO: Check whether RavenDB provides a magic property for Dictionary value collections.
                    result = $"Object.values({ownerExpressionString}).length";

                    return true;
                }

                if (IsSpeciallyTreatedCountPropertyAccess(propertyInfo))
                {
                    var ownerExpressionString = ExpressionParser.CreateJsScriptFromExpression(memberExpression.Expression, parameters);

                    result = $"{ownerExpressionString}.length";
                    return true;
                }
            }

            result = default;
            return false;
        }

        private bool IsDictionaryKeysCollectionCountAccess(MemberExpression memberExpression, PropertyInfo propertyInfo)
        {
            if (propertyInfo.Name != "Count")
            {
                return false;
            }

            if (memberExpression.Expression is MemberExpression parentMemberExpression)
            {
                if (parentMemberExpression.Member is PropertyInfo parentPropertyInfo)
                {
                    if (parentPropertyInfo.Name != "Keys")
                    {
                        return false;
                    }

                    var memberOwnerType = parentPropertyInfo.DeclaringType;
                    if (memberOwnerType.IsGenericType)
                    {
                        memberOwnerType = memberOwnerType.GetGenericTypeDefinition();
                    }

                    if (memberOwnerType == typeof(Dictionary<,>))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool IsDictionaryValuesCollectionCountAccess(MemberExpression memberExpression, PropertyInfo propertyInfo)
        {
            if (propertyInfo.Name != "Count")
            {
                return false;
            }

            if (memberExpression.Expression is MemberExpression parentMemberExpression)
            {
                if (parentMemberExpression.Member is PropertyInfo parentPropertyInfo)
                {
                    if (parentPropertyInfo.Name != "Values")
                    {
                        return false;
                    }

                    var memberOwnerType = parentPropertyInfo.DeclaringType;
                    if (memberOwnerType.IsGenericType)
                    {
                        memberOwnerType = memberOwnerType.GetGenericTypeDefinition();
                    }

                    if (memberOwnerType == typeof(Dictionary<,>))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool IsSpeciallyTreatedCountPropertyAccess(PropertyInfo propertyInfo)
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