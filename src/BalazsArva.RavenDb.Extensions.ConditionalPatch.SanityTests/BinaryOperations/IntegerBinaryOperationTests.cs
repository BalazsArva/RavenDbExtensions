using System;
using System.Linq.Expressions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.Abstractions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Factories;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.SanityTests.TestDocuments;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Utilitites;
using NUnit.Framework;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.SanityTests.BinaryOperations
{
    [TestFixture]
    public class IntegerBinaryOperationTests
    {
        private IExpressionProcessorPipeline expressionProcessorPipeline;

        [SetUp]
        public void Setup()
        {
            expressionProcessorPipeline = ExpressionProcessorPipelineFactory.CreateExpressionProcessorPipeline();
        }

        [Test]
        public void BinaryOps_Integers_Add()
        {
            var result = GetParsedJavaScript(doc => doc.SomeInt + 1 > 0);

            Assert.AreEqual(
                "((this.SomeInt + args.__param1) > args.__param2)",
                result.script);

            Assert.AreEqual(2, result.parameters.Count);
            Assert.AreEqual(1, result.parameters["__param1"]);
            Assert.AreEqual(0, result.parameters["__param2"]);
        }

        [Test]
        public void BinaryOps_Integers_BinaryAnd()
        {
            var result = GetParsedJavaScript(doc => (doc.SomeInt & 1) > 0);

            Assert.AreEqual(
                "((this.SomeInt & args.__param1) > args.__param2)",
                result.script);

            Assert.AreEqual(2, result.parameters.Count);
            Assert.AreEqual(1, result.parameters["__param1"]);
            Assert.AreEqual(0, result.parameters["__param2"]);
        }

        [Test]
        public void BinaryOps_Integers_Division()
        {
            var result = GetParsedJavaScript(doc => doc.SomeInt / 1 > 0);

            Assert.AreEqual(
                "((this.SomeInt / args.__param1) > args.__param2)",
                result.script);

            Assert.AreEqual(2, result.parameters.Count);
            Assert.AreEqual(1, result.parameters["__param1"]);
            Assert.AreEqual(0, result.parameters["__param2"]);
        }

        [Test]
        public void BinaryOps_Integers_ExclusiveOr()
        {
            var result = GetParsedJavaScript(doc => (doc.SomeInt ^ 1) > 0);

            Assert.AreEqual(
                "((this.SomeInt ^ args.__param1) > args.__param2)",
                result.script);

            Assert.AreEqual(2, result.parameters.Count);
            Assert.AreEqual(1, result.parameters["__param1"]);
            Assert.AreEqual(0, result.parameters["__param2"]);
        }

        [Test]
        public void BinaryOps_Integers_Modulo()
        {
            var result = GetParsedJavaScript(doc => doc.SomeInt % 1 > 0);

            Assert.AreEqual(
                "((this.SomeInt % args.__param1) > args.__param2)",
                result.script);

            Assert.AreEqual(2, result.parameters.Count);
            Assert.AreEqual(1, result.parameters["__param1"]);
            Assert.AreEqual(0, result.parameters["__param2"]);
        }

        [Test]
        public void BinaryOps_Integers_Multiply()
        {
            var result = GetParsedJavaScript(doc => doc.SomeInt * 1 > 0);

            Assert.AreEqual(
                "((this.SomeInt * args.__param1) > args.__param2)",
                result.script);

            Assert.AreEqual(2, result.parameters.Count);
            Assert.AreEqual(1, result.parameters["__param1"]);
            Assert.AreEqual(0, result.parameters["__param2"]);
        }

        [Test]
        public void BinaryOps_Integers_BinaryOr()
        {
            var result = GetParsedJavaScript(doc => (doc.SomeInt | 1) > 0);

            Assert.AreEqual(
                "((this.SomeInt | args.__param1) > args.__param2)",
                result.script);

            Assert.AreEqual(2, result.parameters.Count);
            Assert.AreEqual(1, result.parameters["__param1"]);
            Assert.AreEqual(0, result.parameters["__param2"]);
        }

        [Test]
        public void BinaryOps_Integers_Subtract()
        {
            var result = GetParsedJavaScript(doc => doc.SomeInt - 1 > 0);

            Assert.AreEqual(
                "((this.SomeInt - args.__param1) > args.__param2)",
                result.script);

            Assert.AreEqual(2, result.parameters.Count);
            Assert.AreEqual(1, result.parameters["__param1"]);
            Assert.AreEqual(0, result.parameters["__param2"]);
        }

        [Test]
        public void BinaryOps_Integers_Assign()
        {
            var memberExpression = LambdaExpression(doc => doc.SomeInt).Body;
            var constantExpression = Expression.Constant(1);
            var expression = Expression.Assign(memberExpression, constantExpression);

            var parameters = new ScriptParameterDictionary();

            var result = expressionProcessorPipeline.ProcessExpression(expression, parameters);

            Assert.AreEqual(
                "(doc.SomeInt = args.__param1)",
                result);

            Assert.AreEqual(1, parameters.Count);
            Assert.AreEqual(1, parameters["__param1"]);
        }

        [Test]
        public void BinaryOps_Integers_AddAssign()
        {
            var memberExpression = LambdaExpression(doc => doc.SomeInt).Body;
            var constantExpression = Expression.Constant(1);
            var expression = Expression.AddAssign(memberExpression, constantExpression);

            var parameters = new ScriptParameterDictionary();

            var result = expressionProcessorPipeline.ProcessExpression(expression, parameters);

            Assert.AreEqual(
                "(doc.SomeInt += args.__param1)",
                result);

            Assert.AreEqual(1, parameters.Count);
            Assert.AreEqual(1, parameters["__param1"]);
        }

        [Test]
        public void BinaryOps_Integers_AndAssign()
        {
            var memberExpression = LambdaExpression(doc => doc.SomeInt).Body;
            var constantExpression = Expression.Constant(1);
            var expression = Expression.AndAssign(memberExpression, constantExpression);

            var parameters = new ScriptParameterDictionary();

            var result = expressionProcessorPipeline.ProcessExpression(expression, parameters);

            Assert.AreEqual(
                "(doc.SomeInt &= args.__param1)",
                result);

            Assert.AreEqual(1, parameters.Count);
            Assert.AreEqual(1, parameters["__param1"]);
        }

        [Test]
        public void BinaryOps_Integers_DivideAssign()
        {
            var memberExpression = LambdaExpression(doc => doc.SomeInt).Body;
            var constantExpression = Expression.Constant(1);
            var expression = Expression.DivideAssign(memberExpression, constantExpression);

            var parameters = new ScriptParameterDictionary();

            var result = expressionProcessorPipeline.ProcessExpression(expression, parameters);

            Assert.AreEqual(
                "(doc.SomeInt /= args.__param1)",
                result);

            Assert.AreEqual(1, parameters.Count);
            Assert.AreEqual(1, parameters["__param1"]);
        }

        [Test]
        public void BinaryOps_Integers_ExclusiveOrAssign()
        {
            var memberExpression = LambdaExpression(doc => doc.SomeInt).Body;
            var constantExpression = Expression.Constant(1);
            var expression = Expression.ExclusiveOrAssign(memberExpression, constantExpression);

            var parameters = new ScriptParameterDictionary();

            var result = expressionProcessorPipeline.ProcessExpression(expression, parameters);

            Assert.AreEqual(
                "(doc.SomeInt ^= args.__param1)",
                result);

            Assert.AreEqual(1, parameters.Count);
            Assert.AreEqual(1, parameters["__param1"]);
        }

        [Test]
        public void BinaryOps_Integers_ModuloAssign()
        {
            var memberExpression = LambdaExpression(doc => doc.SomeInt).Body;
            var constantExpression = Expression.Constant(1);
            var expression = Expression.ModuloAssign(memberExpression, constantExpression);

            var parameters = new ScriptParameterDictionary();

            var result = expressionProcessorPipeline.ProcessExpression(expression, parameters);

            Assert.AreEqual(
                "(doc.SomeInt %= args.__param1)",
                result);

            Assert.AreEqual(1, parameters.Count);
            Assert.AreEqual(1, parameters["__param1"]);
        }

        [Test]
        public void BinaryOps_Integers_MultiplyAssign()
        {
            var memberExpression = LambdaExpression(doc => doc.SomeInt).Body;
            var constantExpression = Expression.Constant(1);
            var expression = Expression.MultiplyAssign(memberExpression, constantExpression);

            var parameters = new ScriptParameterDictionary();

            var result = expressionProcessorPipeline.ProcessExpression(expression, parameters);

            Assert.AreEqual(
                "(doc.SomeInt *= args.__param1)",
                result);

            Assert.AreEqual(1, parameters.Count);
            Assert.AreEqual(1, parameters["__param1"]);
        }

        [Test]
        public void BinaryOps_Integers_OrAssign()
        {
            var memberExpression = LambdaExpression(doc => doc.SomeInt).Body;
            var constantExpression = Expression.Constant(1);
            var expression = Expression.OrAssign(memberExpression, constantExpression);

            var parameters = new ScriptParameterDictionary();

            var result = expressionProcessorPipeline.ProcessExpression(expression, parameters);

            Assert.AreEqual(
                "(doc.SomeInt |= args.__param1)",
                result);

            Assert.AreEqual(1, parameters.Count);
            Assert.AreEqual(1, parameters["__param1"]);
        }

        [Test]
        public void BinaryOps_Integers_SubtractAssign()
        {
            var memberExpression = LambdaExpression(doc => doc.SomeInt).Body;
            var constantExpression = Expression.Constant(1);
            var expression = Expression.SubtractAssign(memberExpression, constantExpression);

            var parameters = new ScriptParameterDictionary();

            var result = expressionProcessorPipeline.ProcessExpression(expression, parameters);

            Assert.AreEqual(
                "(doc.SomeInt -= args.__param1)",
                result);

            Assert.AreEqual(1, parameters.Count);
            Assert.AreEqual(1, parameters["__param1"]);
        }

        [Test]
        public void BinaryOps_Integers_LessThan()
        {
            var result = GetParsedJavaScript(doc => doc.SomeInt < 0);

            Assert.AreEqual(
                "(this.SomeInt < args.__param1)",
                result.script);

            Assert.AreEqual(1, result.parameters.Count);
            Assert.AreEqual(0, result.parameters["__param1"]);
        }

        [Test]
        public void BinaryOps_Integers_LessThanOrEqualTo()
        {
            var result = GetParsedJavaScript(doc => doc.SomeInt <= 0);

            Assert.AreEqual(
                "(this.SomeInt <= args.__param1)",
                result.script);

            Assert.AreEqual(1, result.parameters.Count);
            Assert.AreEqual(0, result.parameters["__param1"]);
        }

        [Test]
        public void BinaryOps_Integers_GreaterThan()
        {
            var result = GetParsedJavaScript(doc => doc.SomeInt > 0);

            Assert.AreEqual(
                "(this.SomeInt > args.__param1)",
                result.script);

            Assert.AreEqual(1, result.parameters.Count);
            Assert.AreEqual(0, result.parameters["__param1"]);
        }

        [Test]
        public void BinaryOps_Integers_GreaterThanOrEqualTo()
        {
            var result = GetParsedJavaScript(doc => doc.SomeInt >= 0);

            Assert.AreEqual(
                "(this.SomeInt >= args.__param1)",
                result.script);

            Assert.AreEqual(1, result.parameters.Count);
            Assert.AreEqual(0, result.parameters["__param1"]);
        }

        [Test]
        public void BinaryOps_Integers_EqualTo()
        {
            var result = GetParsedJavaScript(doc => doc.SomeInt == 0);

            Assert.AreEqual(
                "(this.SomeInt == args.__param1)",
                result.script);

            Assert.AreEqual(1, result.parameters.Count);
            Assert.AreEqual(0, result.parameters["__param1"]);
        }

        [Test]
        public void BinaryOps_Integers_NotEqualTo()
        {
            var result = GetParsedJavaScript(doc => doc.SomeInt != 0);

            Assert.AreEqual(
                "(this.SomeInt != args.__param1)",
                result.script);

            Assert.AreEqual(1, result.parameters.Count);
            Assert.AreEqual(0, result.parameters["__param1"]);
        }

        private (string script, ScriptParameterDictionary parameters) GetParsedJavaScript(Expression<Func<TestDocument, bool>> expression)
        {
            var patchScriptConditionBuilder = PatchScriptConditionBuilderFactory.CreatePatchScriptBodyBuilder();

            var parameters = new ScriptParameterDictionary();
            var script = patchScriptConditionBuilder.CreateScriptCondition(expression, parameters);

            return (script, parameters);
        }

        private static LambdaExpression LambdaExpression<TProperty>(Expression<Func<TestDocument, TProperty>> expression)
        {
            // Convenience method for not having to explicitly write
            // ((Expression<Func<TestDocument, int[]>>)(doc => doc.Array))-like things everywhere.

            return expression;
        }
    }
}