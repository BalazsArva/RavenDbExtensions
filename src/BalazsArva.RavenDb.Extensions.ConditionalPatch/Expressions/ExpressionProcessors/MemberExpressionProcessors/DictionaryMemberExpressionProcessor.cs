using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.Abstractions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Utilitites;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.ExpressionProcessors.MemberExpressionProcessors
{
    public class DictionaryMemberExpressionProcessor : IExpressionProcessor<MemberExpression>
    {
        private const string CountPropertyName = "Count";
        private const string DictionaryKeysPropertyName = "Keys";
        private const string DictionaryValuesPropertyName = "Values";

        private static readonly Type GenericIDictionaryInterfaceType = typeof(IDictionary<,>);
        private static readonly Type GenericDictionaryImplementationType = typeof(Dictionary<,>);

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
                if (IsDictionaryCountAccess(propertyInfo))
                {
                    // Dictionaries are mapped to object literals based on their keys when they are serialized to JSON,
                    // so we can't use the .length as we would with a regular collection. But there are as many items
                    // in a dictionary as there are keys, so we need find the number of keys to resolve the expression.
                    var ownerExpressionString = ExpressionParser.CreateJsScriptFromExpression(memberExpression.Expression, parameters);

                    // TODO: Check whether RavenDB provides a magic property for Dictionary key collections.
                    result = $"Object.keys({ownerExpressionString}).length";
                    return true;
                }

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
            }

            result = default;
            return false;
        }

        private bool IsDictionaryCountAccess(PropertyInfo propertyInfo)
        {
            if (propertyInfo.Name != CountPropertyName)
            {
                return false;
            }

            // TODO: Write tests where we check that this works for any IDictionary<TKey, TValue> implementation.
            return propertyInfo
                .DeclaringType
                .GetInterfaces()
                .Select(i => i.IsGenericType ? i.GetGenericTypeDefinition() : i)
                .Any(i => i == GenericIDictionaryInterfaceType);
        }

        private bool IsDictionaryKeysCollectionCountAccess(MemberExpression memberExpression, PropertyInfo propertyInfo)
        {
            if (propertyInfo.Name != CountPropertyName)
            {
                return false;
            }

            if (memberExpression.Expression is MemberExpression parentMemberExpression &&
                parentMemberExpression.Member is PropertyInfo parentPropertyInfo)
            {
                if (parentPropertyInfo.Name != DictionaryKeysPropertyName)
                {
                    return false;
                }

                var memberOwnerType = parentPropertyInfo.DeclaringType;
                if (memberOwnerType.IsGenericType)
                {
                    memberOwnerType = memberOwnerType.GetGenericTypeDefinition();
                }

                // Unlike in IsDictionaryCountAccess, here we shouldn't check against the interface but the implementation
                // because the Keys collection is specific to the System.Collections.Generic.Dictionary<TKey, TValue>
                // implementation (it is a nested class of it).
                // TODO: Write tests where we check that this only works for the Dictionary<TKey, TValue> implementation and not an arbitrary IDictionary<TKey, TValue>
                if (memberOwnerType == GenericDictionaryImplementationType)
                {
                    return true;
                }
            }

            return false;
        }

        private bool IsDictionaryValuesCollectionCountAccess(MemberExpression memberExpression, PropertyInfo propertyInfo)
        {
            if (propertyInfo.Name != CountPropertyName)
            {
                return false;
            }

            if (memberExpression.Expression is MemberExpression parentMemberExpression &&
                parentMemberExpression.Member is PropertyInfo parentPropertyInfo)
            {
                if (parentPropertyInfo.Name != DictionaryValuesPropertyName)
                {
                    return false;
                }

                var memberOwnerType = parentPropertyInfo.DeclaringType;
                if (memberOwnerType.IsGenericType)
                {
                    memberOwnerType = memberOwnerType.GetGenericTypeDefinition();
                }

                // Unlike in IsDictionaryCountAccess, here we shouldn't check against the interface but the implementation
                // because the Values collection is specific to the System.Collections.Generic.Dictionary<TKey, TValue>
                // implementation (it is a nested class of it).
                // TODO: Write tests where we check that this only works for the Dictionary<TKey, TValue> implementation and not an arbitrary IDictionary<TKey, TValue>
                if (memberOwnerType == GenericDictionaryImplementationType)
                {
                    return true;
                }
            }

            return false;
        }
    }
}