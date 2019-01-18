using System;
using System.Linq.Expressions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.SanityTests.TestDocuments;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Utilitites;
using NUnit.Framework;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.SanityTests.Regression
{
    [TestFixture]
    public class RegressionTests
    {
        [Test]
        public void Predicate_String_TestAgainstNull()
        {
            var result = GetParsedJavaScript(doc => doc.SomeString != null);

            Assert.AreEqual("(this.SomeString != args.__param1)", result.script);
            Assert.AreEqual(1, result.parameters.Count);
            Assert.IsNull(result.parameters["__param1"]);
        }

        [Test]
        public void Predicate_String_TestAgainstEmptyString()
        {
            var result = GetParsedJavaScript(doc => doc.SomeString != string.Empty);

            Assert.AreEqual("(this.SomeString != args.__param1)", result.script);
            Assert.AreEqual(1, result.parameters.Count);
            Assert.AreEqual(string.Empty, result.parameters["__param1"]);
        }

        [Test]
        public void Predicate_String_TestAgainstEmptyStringLiteral()
        {
            var result = GetParsedJavaScript(doc => doc.SomeString != "");

            Assert.AreEqual("(this.SomeString != args.__param1)", result.script);
            Assert.AreEqual(1, result.parameters.Count);
            Assert.AreEqual(string.Empty, result.parameters["__param1"]);
        }

        [Test]
        public void Predicate_String_TestAgainstWhiteSpaceStringLiteral()
        {
            var result = GetParsedJavaScript(doc => doc.SomeString != " ");

            Assert.AreEqual("(this.SomeString != args.__param1)", result.script);
            Assert.AreEqual(1, result.parameters.Count);
            Assert.AreEqual(" ", result.parameters["__param1"]);
        }

        [Test]
        public void Predicate_String_TestAgainstNonEmptyNonNullNonWhiteSpaceStringLiteral()
        {
            var result = GetParsedJavaScript(doc => doc.SomeString != "a");

            Assert.AreEqual("(this.SomeString != args.__param1)", result.script);
            Assert.AreEqual(1, result.parameters.Count);
            Assert.AreEqual("a", result.parameters["__param1"]);
        }

        [Test]
        public void Predicate_String_TestingWithStringIsNullOrEmpty()
        {
            var result = GetParsedJavaScript(doc => string.IsNullOrEmpty(doc.SomeString));

            Assert.AreEqual("(this.SomeString == null || this.SomeString == '')", result.script);
            Assert.AreEqual(0, result.parameters.Count);
        }

        [Test]
        public void Predicate_String_TestingWithStringIsNullOrWhiteSpace()
        {
            var result = GetParsedJavaScript(doc => string.IsNullOrWhiteSpace(doc.SomeString));

            Assert.AreEqual("(this.SomeString == null || this.SomeString.trim() == '')", result.script);
            Assert.AreEqual(0, result.parameters.Count);
        }

        private (string script, ScriptParameterDictionary parameters) GetParsedJavaScript<TProperty>(Expression<Func<TestDocument, TProperty>> expression)
        {
            var parameters = new ScriptParameterDictionary();
            var script = ExpressionParser.CreateJsScriptFromExpression(expression, parameters);

            return (script, parameters);
        }
    }
}