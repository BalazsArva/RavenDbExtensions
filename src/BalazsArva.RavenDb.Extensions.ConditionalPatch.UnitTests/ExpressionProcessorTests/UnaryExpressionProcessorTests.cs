using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.ExpressionProcessors;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.UnitTests.TestDocuments;
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
            Assert.Throws<ArgumentNullException>(() => processor.TryProcess(null, out var _));
        }

        [TestCaseSource(nameof(ArrayLengthTestCases))]
        public void TryProcess_ArrayLengthOfParameter_ReturnsTrueAndCreatesCorrectScript(Expression arrayExpression, string expectedScript)
        {
            var expression = Expression.ArrayLength(arrayExpression);

            var success = processor.TryProcess(expression, out var resultScript);

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