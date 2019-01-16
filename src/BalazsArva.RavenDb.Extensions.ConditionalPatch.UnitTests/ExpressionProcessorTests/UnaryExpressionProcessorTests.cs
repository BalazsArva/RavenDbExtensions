using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.ExpressionProcessors;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.UnitTests.TestDocuments;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Utilitites;
using NUnit.Framework;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.UnitTests.ExpressionProcessorTests
{
    [TestFixture]
    public class UnaryExpressionProcessorTests
    {
        private UnaryExpressionProcessor processor;

        [SetUp]
        public void Setup()
        {
            processor = new UnaryExpressionProcessor();
        }

        [Test]
        public void TryProcess_ExpressionIsNull_ThrowsArgumentNullException()
        {
            var exceptionThrown = Assert.Throws<ArgumentNullException>(() => processor.TryProcess(null, new ScriptParameterDictionary(), out var _));

            Assert.AreEqual("expression", exceptionThrown.ParamName);
        }

        [Test]
        public void TryProcess_ParameterDictionaryIsNull_ThrowsArgumentNullException()
        {
            var expression = Expression.ArrayLength(LambdaExpression(doc => doc.Array).Body);

            var exceptionThrown = Assert.Throws<ArgumentNullException>(() => processor.TryProcess(expression, null, out var _));

            Assert.AreEqual("parameters", exceptionThrown.ParamName);
        }

        [TestCaseSource(nameof(ArrayLengthTestCases))]
        public void TryProcess_ArrayLengthOfParameter_ReturnsTrueAndCreatesCorrectScript(Expression arrayExpression, string expectedScript)
        {
            var expression = Expression.ArrayLength(arrayExpression);

            var success = processor.TryProcess(expression, new ScriptParameterDictionary(), out var resultScript);

            Assert.IsTrue(success);
            Assert.AreEqual(expectedScript, resultScript);
        }

        private static IEnumerable<object[]> ArrayLengthTestCases()
        {
            yield return new object[]
            {
                LambdaExpression(doc => doc.Array).Body,
                "this.Array.length"
            };
        }

        private static LambdaExpression LambdaExpression<TProperty>(Expression<Func<TestDocument, TProperty>> expression)
        {
            // Convenience method for not having to explicitly write
            // ((Expression<Func<TestDocument, int[]>>)(doc => doc.Array))-like things everywhere.

            return expression;
        }
    }
}