using System;
using System.Linq.Expressions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Factories;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.SanityTests.TestDocuments;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Utilitites;
using NUnit.Framework;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.SanityTests.MemberAccess
{
    [TestFixture]
    public class NullableTypeMemberAccessTests
    {
        [Test]
        public void Predicate_Nullable_TestAgainstNull()
        {
            var result = GetParsedJavaScript(doc => doc.SomeNullableLong != null);

            Assert.AreEqual("(doc.SomeNullableLong != args.__param1)", result.script);
            Assert.AreEqual(1, result.parameters.Count);
            Assert.IsNull(result.parameters["__param1"]);
        }

        [Test]
        public void Predicate_Nullable_TestHasValue()
        {
            var result = GetParsedJavaScript(doc => doc.SomeNullableLong.HasValue);

            Assert.AreEqual("(doc.SomeNullableLong != null)", result.script);
            Assert.AreEqual(0, result.parameters.Count);
        }

        [Test]
        public void Predicate_Nullable_TestValueResolution()
        {
            var result = GetParsedJavaScript(doc => doc.SomeNullableLong != null);

            Assert.AreEqual("(doc.SomeNullableLong != args.__param1)", result.script);
            Assert.AreEqual(1, result.parameters.Count);
            Assert.IsNull(result.parameters["__param1"]);
        }

        [Test]
        public void Predicate_Nullable_TestValueAccessTestAgainstNull()
        {
#pragma warning disable CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
            var result = GetParsedJavaScript(doc => doc.SomeNullableLong.Value != null);
#pragma warning restore CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'

            Assert.AreEqual("(doc.SomeNullableLong != args.__param1)", result.script);
            Assert.AreEqual(1, result.parameters.Count);
            Assert.IsNull(result.parameters["__param1"]);
        }

        [Test]
        public void Predicate_Nullable_TestValueAccessWithoutDotValue()
        {
            var result = GetParsedJavaScript(doc => doc.SomeNullableLong > 0);

            // TODO: This is failing because of Nullable and Non-nullable comparison, figure out what to do with it. Maybe should give special treatment in the binary processor and insert a non-null check + value access expression pair.
            Assert.AreEqual("(doc.SomeNullableLong > args.__param1)", result.script);
            Assert.AreEqual(1, result.parameters.Count);
            Assert.AreEqual(0, result.parameters["__param1"]);
        }

        private (string script, ScriptParameterDictionary parameters) GetParsedJavaScript<TProperty>(Expression<Func<TestDocument, TProperty>> expression)
        {
            var processor = ExpressionProcessorPipelineFactory.CreateExpressionProcessorPipeline();

            var parameters = new ScriptParameterDictionary();
            var script = processor.ProcessExpression(expression, parameters);

            return (script, parameters);
        }
    }
}