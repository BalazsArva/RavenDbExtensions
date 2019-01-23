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

            Assert.AreEqual("(this.SomeNullableLong != args.__param1)", result.script);
            Assert.AreEqual(1, result.parameters.Count);
            Assert.IsNull(result.parameters["__param1"]);
        }

        [Test]
        public void Predicate_Nullable_TestHasValue()
        {
            var result = GetParsedJavaScript(doc => doc.SomeNullableLong.HasValue);

            Assert.AreEqual("(this.SomeNullableLong != null)", result.script);
            Assert.AreEqual(0, result.parameters.Count);
        }

        [Test]
        public void Predicate_Nullable_TestValueResolution()
        {
            var result = GetParsedJavaScript(doc => doc.SomeNullableLong != null);

            Assert.AreEqual("(this.SomeNullableLong != args.__param1)", result.script);
            Assert.AreEqual(1, result.parameters.Count);
            Assert.IsNull(result.parameters["__param1"]);
        }

        [Test]
        public void Predicate_Nullable_TestValueAccessTestAgainstNull()
        {
#pragma warning disable CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'
            var result = GetParsedJavaScript(doc => doc.SomeNullableLong.Value != null);
#pragma warning restore CS0472 // The result of the expression is always the same since a value of this type is never equal to 'null'

            Assert.AreEqual("(this.SomeNullableLong != args.__param1)", result.script);
            Assert.AreEqual(1, result.parameters.Count);
            Assert.IsNull(result.parameters["__param1"]);
        }

        [Test]
        public void Predicate_Nullable_TestValueAccessWithoutDotValue_LeftOperandIsNullable()
        {
            var result = GetParsedJavaScript(doc => doc.SomeNullableLong > 0);
            
            Assert.AreEqual("((this.SomeNullableLong != args.__param1) ? (this.SomeNullableLong > args.__param2) : args.__param3)", result.script);
            Assert.AreEqual(3, result.parameters.Count);
            Assert.AreEqual(null, result.parameters["__param1"]);
            Assert.AreEqual(0, result.parameters["__param2"]);
            Assert.AreEqual(false, result.parameters["__param3"]);
        }

        [Test]
        public void Predicate_Nullable_TestValueAccessWithoutDotValue_RightOperandIsNullable()
        {
            var result = GetParsedJavaScript(doc => 0 < doc.SomeNullableLong );

            Assert.AreEqual("((this.SomeNullableLong != args.__param1) ? (args.__param2 < this.SomeNullableLong) : args.__param3)", result.script);
            Assert.AreEqual(3, result.parameters.Count);
            Assert.AreEqual(null, result.parameters["__param1"]);
            Assert.AreEqual(0, result.parameters["__param2"]);
            Assert.AreEqual(false, result.parameters["__param3"]);
        }

        private (string script, ScriptParameterDictionary parameters) GetParsedJavaScript(Expression<Func<TestDocument, bool>> expression)
        {
            var patchScriptConditionBuilder = PatchScriptConditionBuilderFactory.CreatePatchScriptBodyBuilder();

            var parameters = new ScriptParameterDictionary();
            var script = patchScriptConditionBuilder.CreateScriptCondition(expression, parameters);

            return (script, parameters);
        }
    }
}