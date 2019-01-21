using System;
using System.Linq.Expressions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Factories;
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

        [Test]
        public void Predicate_String_TestStartsWithWithStringValueOnly()
        {
            var result = GetParsedJavaScript(doc => doc.SomeString.StartsWith("ab"));

            Assert.AreEqual("this.SomeString.startsWith(args.__param1)", result.script);
            Assert.AreEqual(1, result.parameters.Count);
            Assert.AreEqual("ab", result.parameters["__param1"]);
        }

        [Test]
        public void Predicate_String_TestEndsWithWithStringValueOnly()
        {
            var result = GetParsedJavaScript(doc => doc.SomeString.EndsWith("ab"));

            Assert.AreEqual("this.SomeString.endsWith(args.__param1)", result.script);
            Assert.AreEqual(1, result.parameters.Count);
            Assert.AreEqual("ab", result.parameters["__param1"]);
        }

        [Test]
        public void Predicate_String_TestPadLeftWithTotalWidthOnly()
        {
            const string fiveSpaces = "     ";

            var result = GetParsedJavaScript(doc => doc.SomeString.PadLeft(5) != fiveSpaces);

            Assert.AreEqual("(this.SomeString.padStart(args.__param1, args.__param2) != args.__param3)", result.script);
            Assert.AreEqual(3, result.parameters.Count);
            Assert.AreEqual(5, result.parameters["__param1"]);
            Assert.AreEqual(' ', result.parameters["__param2"]);
            Assert.AreEqual(fiveSpaces, result.parameters["__param3"]);
        }

        [Test]
        public void Predicate_String_TestPadLeftWithTotalWidthAndPaddingChar()
        {
            const string fiveAs = "aaaaa";

            var result = GetParsedJavaScript(doc => doc.SomeString.PadLeft(5, 'a') != fiveAs);

            Assert.AreEqual("(this.SomeString.padStart(args.__param1, args.__param2) != args.__param3)", result.script);
            Assert.AreEqual(3, result.parameters.Count);
            Assert.AreEqual(5, result.parameters["__param1"]);
            Assert.AreEqual('a', result.parameters["__param2"]);
            Assert.AreEqual(fiveAs, result.parameters["__param3"]);
        }

        [Test]
        public void Predicate_String_TestPadRightWithTotalWidthOnly()
        {
            const string fiveSpaces = "     ";

            var result = GetParsedJavaScript(doc => doc.SomeString.PadRight(5) != fiveSpaces);

            Assert.AreEqual("(this.SomeString.padEnd(args.__param1, args.__param2) != args.__param3)", result.script);
            Assert.AreEqual(3, result.parameters.Count);
            Assert.AreEqual(5, result.parameters["__param1"]);
            Assert.AreEqual(' ', result.parameters["__param2"]);
            Assert.AreEqual(fiveSpaces, result.parameters["__param3"]);
        }

        [Test]
        public void Predicate_String_TestPadRightWithTotalWidthAndPaddingChar()
        {
            const string fiveAs = "aaaaa";

            var result = GetParsedJavaScript(doc => doc.SomeString.PadRight(5, 'a') != fiveAs);

            Assert.AreEqual("(this.SomeString.padEnd(args.__param1, args.__param2) != args.__param3)", result.script);
            Assert.AreEqual(3, result.parameters.Count);
            Assert.AreEqual(5, result.parameters["__param1"]);
            Assert.AreEqual('a', result.parameters["__param2"]);
            Assert.AreEqual(fiveAs, result.parameters["__param3"]);
        }

        [Test]
        public void Predicate_String_TestInsert()
        {
            var result = GetParsedJavaScript(doc => doc.SomeString.Insert(5, "abc") != string.Empty);

            Assert.AreEqual("((this.SomeString.substring(0, args.__param1) + args.__param2 + this.SomeString.substring(args.__param1)) != args.__param3)", result.script);
            Assert.AreEqual(3, result.parameters.Count);
            Assert.AreEqual(5, result.parameters["__param1"]);
            Assert.AreEqual("abc", result.parameters["__param2"]);
            Assert.AreEqual(string.Empty, result.parameters["__param3"]);
        }

        [Test]
        public void Predicate_String_TestRemoveWithStartIndex()
        {
            var result = GetParsedJavaScript(doc => doc.SomeString.Remove(5) != string.Empty);

            Assert.AreEqual("(this.SomeString.substring(0, args.__param1) != args.__param2)", result.script);
            Assert.AreEqual(2, result.parameters.Count);
            Assert.AreEqual(5, result.parameters["__param1"]);
            Assert.AreEqual(string.Empty, result.parameters["__param2"]);
        }

        [Test]
        public void Predicate_String_TestRemoveWithStartIndexAndCount()
        {
            var result = GetParsedJavaScript(doc => doc.SomeString.Remove(5, 1) != string.Empty);

            Assert.AreEqual("((this.SomeString.substring(0, args.__param1) + this.SomeString.substring(args.__param2)) != args.__param3)", result.script);
            Assert.AreEqual(3, result.parameters.Count);
            Assert.AreEqual(5, result.parameters["__param1"]);
            Assert.AreEqual(6, result.parameters["__param2"]);
            Assert.AreEqual(string.Empty, result.parameters["__param3"]);
        }

        [Test]
        public void Predicate_String_TestRemoveWithStartIndexAndCount_CountCalculatedFromParameterLength()
        {
            var result = GetParsedJavaScript(doc => doc.SomeString.Remove(doc.SomeString.Length - 2, 1) != string.Empty);

            Assert.AreEqual("((this.SomeString.substring(0, (this.SomeString.length - args.__param1)) + this.SomeString.substring(((this.SomeString.length - args.__param2) + args.__param3))) != args.__param4)", result.script);
            Assert.AreEqual(4, result.parameters.Count);
            Assert.AreEqual(2, result.parameters["__param1"]);
            Assert.AreEqual(2, result.parameters["__param2"]);
            Assert.AreEqual(1, result.parameters["__param3"]);
            Assert.AreEqual(string.Empty, result.parameters["__param4"]);
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