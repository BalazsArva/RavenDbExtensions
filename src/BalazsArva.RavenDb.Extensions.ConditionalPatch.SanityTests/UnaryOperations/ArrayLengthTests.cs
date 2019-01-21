using System;
using System.Linq.Expressions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Factories;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.SanityTests.TestDocuments;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Utilitites;
using NUnit.Framework;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.SanityTests.UnaryOperations
{
    [TestFixture]
    public class ArrayLengthTests
    {
        [Test]
        public void TryProcess_ArrayLengthOfParameter_ReturnsTrueAndCreatesCorrectScript()
        {
            var result = GetParsedJavaScript(doc => doc.SomeIntArray.Length != 0);

            Assert.AreEqual("(this.SomeIntArray.length != args.__param1)", result.script);
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