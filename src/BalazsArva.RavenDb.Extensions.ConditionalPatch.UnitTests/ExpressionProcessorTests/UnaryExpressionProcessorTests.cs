using System;
using System.Linq.Expressions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.Abstractions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Factories;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.UnitTests.TestDocuments;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Utilitites;
using NUnit.Framework;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.UnitTests.ExpressionProcessorTests
{
    [TestFixture]
    public class UnaryExpressionProcessorTests
    {
        private IExpressionProcessorPipeline expressionProcessorPipeline;

        [SetUp]
        public void Setup()
        {
            expressionProcessorPipeline = ExpressionProcessorPipelineFactory.CreateExpressionProcessorPipeline();
        }

        [Test]
        public void TryProcess_ExpressionIsNull_ThrowsArgumentNullException()
        {
            var exceptionThrown = Assert.Throws<ArgumentNullException>(() => expressionProcessorPipeline.ProcessExpression(null, new ScriptParameterDictionary()));

            Assert.AreEqual("expression", exceptionThrown.ParamName);
        }

        [Test]
        public void TryProcess_ParameterDictionaryIsNull_ThrowsArgumentNullException()
        {
            var expression = Expression.ArrayLength(LambdaExpression(doc => doc.Array).Body);

            var exceptionThrown = Assert.Throws<ArgumentNullException>(() => expressionProcessorPipeline.ProcessExpression(expression, null));

            Assert.AreEqual("parameters", exceptionThrown.ParamName);
        }

        [Test]
        public void TryProcess_ArrayLengthOfParameter_ReturnsTrueAndCreatesCorrectScript()
        {
            var result = expressionProcessorPipeline.ProcessExpression(LambdaExpression(doc => doc.Array.Length).Body, new ScriptParameterDictionary());

            Assert.AreEqual("this.Array.length", result);
        }

        private static LambdaExpression LambdaExpression<TProperty>(Expression<Func<TestDocument, TProperty>> expression)
        {
            // Convenience method for not having to explicitly write
            // ((Expression<Func<TestDocument, int[]>>)(doc => doc.Array))-like things everywhere.
            return expression;
        }
    }
}