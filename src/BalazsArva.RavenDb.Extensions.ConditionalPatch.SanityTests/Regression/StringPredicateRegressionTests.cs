﻿using System;
using System.Linq.Expressions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.SanityTests.TestDocuments;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Utilitites;
using NUnit.Framework;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.SanityTests.Regression
{
    [TestFixture]
    public class StringPredicateRegressionTests
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

        [Test]
        public void Predicate_String_TestingWithStringContainsChar()
        {
            var result = GetParsedJavaScript(doc => doc.SomeString.Contains('a'));

            Assert.AreEqual("(this.SomeString.indexOf(args.__param1) != -1)", result.script);
            Assert.AreEqual(1, result.parameters.Count);
            Assert.AreEqual('a', result.parameters["__param1"]);
        }

        [Test]
        public void Predicate_String_TestingWithStringContainsString()
        {
            var result = GetParsedJavaScript(doc => doc.SomeString.Contains("ab"));

            Assert.AreEqual("(this.SomeString.indexOf(args.__param1) != -1)", result.script);
            Assert.AreEqual(1, result.parameters.Count);
            Assert.AreEqual("ab", result.parameters["__param1"]);
        }

        [Test]
        public void Predicate_String_TestTrim()
        {
            var result = GetParsedJavaScript(doc => doc.SomeString.Trim() != string.Empty);

            Assert.AreEqual("(this.SomeString.trim() != args.__param1)", result.script);
            Assert.AreEqual(1, result.parameters.Count);
            Assert.AreEqual(string.Empty, result.parameters["__param1"]);
        }

        [Test]
        public void Predicate_String_TestTrimStart()
        {
            var result = GetParsedJavaScript(doc => doc.SomeString.TrimStart() != string.Empty);

            Assert.AreEqual("(this.SomeString.trimStart() != args.__param1)", result.script);
            Assert.AreEqual(1, result.parameters.Count);
            Assert.AreEqual(string.Empty, result.parameters["__param1"]);
        }

        [Test]
        public void Predicate_String_TestTrimEnd()
        {
            var result = GetParsedJavaScript(doc => doc.SomeString.TrimEnd() != string.Empty);

            Assert.AreEqual("(this.SomeString.trimEnd() != args.__param1)", result.script);
            Assert.AreEqual(1, result.parameters.Count);
            Assert.AreEqual(string.Empty, result.parameters["__param1"]);
        }

        [Test]
        public void Predicate_String_TestToLower()
        {
            var result = GetParsedJavaScript(doc => doc.SomeString.ToLower() != string.Empty);

            Assert.AreEqual("(this.SomeString.toLowerCase() != args.__param1)", result.script);
            Assert.AreEqual(1, result.parameters.Count);
            Assert.AreEqual(string.Empty, result.parameters["__param1"]);
        }

        [Test]
        public void Predicate_String_TestToUpper()
        {
            var result = GetParsedJavaScript(doc => doc.SomeString.ToUpper() != string.Empty);

            Assert.AreEqual("(this.SomeString.toUpperCase() != args.__param1)", result.script);
            Assert.AreEqual(1, result.parameters.Count);
            Assert.AreEqual(string.Empty, result.parameters["__param1"]);
        }

        [Test]
        public void Predicate_String_TestSubstringWithStartIndex()
        {
            var result = GetParsedJavaScript(doc => doc.SomeString.Substring(1) != "a");

            Assert.AreEqual("(this.SomeString.substring(args.__param1) != args.__param2)", result.script);
            Assert.AreEqual(2, result.parameters.Count);
            Assert.AreEqual(1, result.parameters["__param1"]);
            Assert.AreEqual("a", result.parameters["__param2"]);
        }

        [Test]
        public void Predicate_String_TestSubstringWithStartIndexAndLength()
        {
            var result = GetParsedJavaScript(doc => doc.SomeString.Substring(1, 2) != "ab");

            Assert.AreEqual("(this.SomeString.substring(args.__param1, args.__param2) != args.__param3)", result.script);
            Assert.AreEqual(3, result.parameters.Count);
            Assert.AreEqual(1, result.parameters["__param1"]);
            Assert.AreEqual(2, result.parameters["__param2"]);
            Assert.AreEqual("ab", result.parameters["__param3"]);
        }

        private (string script, ScriptParameterDictionary parameters) GetParsedJavaScript<TProperty>(Expression<Func<TestDocument, TProperty>> expression)
        {
            var parameters = new ScriptParameterDictionary();
            var script = ExpressionParser.CreateJsScriptFromExpression(expression, parameters);

            return (script, parameters);
        }
    }
}