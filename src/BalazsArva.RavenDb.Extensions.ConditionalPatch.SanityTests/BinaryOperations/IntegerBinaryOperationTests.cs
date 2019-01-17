using System;
using System.Linq.Expressions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.SanityTests.TestDocuments;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Utilitites;
using NUnit.Framework;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.SanityTests.BinaryOperations
{
    [TestFixture]
    public class IntegerBinaryOperationTests
    {
        [Test]
        public void BinaryOps_Integers_Add()
        {
            var expression = LambdaExpression(doc => doc.SomeInt + 1);
            var parameters = new ScriptParameterDictionary();

            var result = ExpressionParser.CreateJsScriptFromExpression(expression, parameters);

            Assert.AreEqual(
                "(this.SomeInt + args.__param1)",
                result);

            Assert.AreEqual(1, parameters.Count);
            Assert.AreEqual(1, parameters["__param1"]);
        }

        [Test]
        public void BinaryOps_Integers_BinaryAnd()
        {
            var expression = LambdaExpression(doc => doc.SomeInt & 1);
            var parameters = new ScriptParameterDictionary();

            var result = ExpressionParser.CreateJsScriptFromExpression(expression, parameters);

            Assert.AreEqual(
                "(this.SomeInt & args.__param1)",
                result);

            Assert.AreEqual(1, parameters.Count);
            Assert.AreEqual(1, parameters["__param1"]);
        }

        [Test]
        public void BinaryOps_Integers_Division()
        {
            var expression = LambdaExpression(doc => doc.SomeInt / 1);
            var parameters = new ScriptParameterDictionary();

            var result = ExpressionParser.CreateJsScriptFromExpression(expression, parameters);

            Assert.AreEqual(
                "(this.SomeInt / args.__param1)",
                result);

            Assert.AreEqual(1, parameters.Count);
            Assert.AreEqual(1, parameters["__param1"]);
        }

        [Test]
        public void BinaryOps_Integers_ExclusiveOr()
        {
            var expression = LambdaExpression(doc => doc.SomeInt ^ 1);
            var parameters = new ScriptParameterDictionary();

            var result = ExpressionParser.CreateJsScriptFromExpression(expression, parameters);

            Assert.AreEqual(
                "(this.SomeInt ^ args.__param1)",
                result);

            Assert.AreEqual(1, parameters.Count);
            Assert.AreEqual(1, parameters["__param1"]);
        }

        [Test]
        public void BinaryOps_Integers_Modulo()
        {
            var expression = LambdaExpression(doc => doc.SomeInt % 1);
            var parameters = new ScriptParameterDictionary();

            var result = ExpressionParser.CreateJsScriptFromExpression(expression, parameters);

            Assert.AreEqual(
                "(this.SomeInt % args.__param1)",
                result);

            Assert.AreEqual(1, parameters.Count);
            Assert.AreEqual(1, parameters["__param1"]);
        }

        [Test]
        public void BinaryOps_Integers_Multiply()
        {
            var expression = LambdaExpression(doc => doc.SomeInt * 1);
            var parameters = new ScriptParameterDictionary();

            var result = ExpressionParser.CreateJsScriptFromExpression(expression, parameters);

            Assert.AreEqual(
                "(this.SomeInt * args.__param1)",
                result);

            Assert.AreEqual(1, parameters.Count);
            Assert.AreEqual(1, parameters["__param1"]);
        }

        [Test]
        public void BinaryOps_Integers_BinaryOr()
        {
            var expression = LambdaExpression(doc => doc.SomeInt | 1);
            var parameters = new ScriptParameterDictionary();

            var result = ExpressionParser.CreateJsScriptFromExpression(expression, parameters);

            Assert.AreEqual(
                "(this.SomeInt | args.__param1)",
                result);

            Assert.AreEqual(1, parameters.Count);
            Assert.AreEqual(1, parameters["__param1"]);
        }

        [Test]
        public void BinaryOps_Integers_Subtract()
        {
            var expression = LambdaExpression(doc => doc.SomeInt - 1);
            var parameters = new ScriptParameterDictionary();

            var result = ExpressionParser.CreateJsScriptFromExpression(expression, parameters);

            Assert.AreEqual(
                "(this.SomeInt - args.__param1)",
                result);

            Assert.AreEqual(1, parameters.Count);
            Assert.AreEqual(1, parameters["__param1"]);
        }

        [Test]
        public void BinaryOps_Integers_Assign()
        {
            var memberExpression = LambdaExpression(doc => doc.SomeInt).Body;
            var constantExpression = Expression.Constant(1);
            var expression = Expression.Assign(memberExpression, constantExpression);

            var parameters = new ScriptParameterDictionary();

            var result = ExpressionParser.CreateJsScriptFromExpression(expression, parameters);

            Assert.AreEqual(
                "(this.SomeInt = args.__param1)",
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

            var result = ExpressionParser.CreateJsScriptFromExpression(expression, parameters);

            Assert.AreEqual(
                "(this.SomeInt += args.__param1)",
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

            var result = ExpressionParser.CreateJsScriptFromExpression(expression, parameters);

            Assert.AreEqual(
                "(this.SomeInt &= args.__param1)",
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

            var result = ExpressionParser.CreateJsScriptFromExpression(expression, parameters);

            Assert.AreEqual(
                "(this.SomeInt /= args.__param1)",
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

            var result = ExpressionParser.CreateJsScriptFromExpression(expression, parameters);

            Assert.AreEqual(
                "(this.SomeInt ^= args.__param1)",
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

            var result = ExpressionParser.CreateJsScriptFromExpression(expression, parameters);

            Assert.AreEqual(
                "(this.SomeInt %= args.__param1)",
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

            var result = ExpressionParser.CreateJsScriptFromExpression(expression, parameters);

            Assert.AreEqual(
                "(this.SomeInt *= args.__param1)",
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

            var result = ExpressionParser.CreateJsScriptFromExpression(expression, parameters);

            Assert.AreEqual(
                "(this.SomeInt |= args.__param1)",
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

            var result = ExpressionParser.CreateJsScriptFromExpression(expression, parameters);

            Assert.AreEqual(
                "(this.SomeInt -= args.__param1)",
                result);

            Assert.AreEqual(1, parameters.Count);
            Assert.AreEqual(1, parameters["__param1"]);
        }

        [Test]
        public void BinaryOps_Integers_LessThan()
        {
            var expression = LambdaExpression(doc => doc.SomeInt < 0);
            var parameters = new ScriptParameterDictionary();

            var result = ExpressionParser.CreateJsScriptFromExpression(expression, parameters);

            Assert.AreEqual(
                "(this.SomeInt < args.__param1)",
                result);

            Assert.AreEqual(1, parameters.Count);
            Assert.AreEqual(0, parameters["__param1"]);
        }

        [Test]
        public void BinaryOps_Integers_LessThanOrEqualTo()
        {
            var expression = LambdaExpression(doc => doc.SomeInt <= 0);
            var parameters = new ScriptParameterDictionary();

            var result = ExpressionParser.CreateJsScriptFromExpression(expression, parameters);

            Assert.AreEqual(
                "(this.SomeInt <= args.__param1)",
                result);

            Assert.AreEqual(1, parameters.Count);
            Assert.AreEqual(0, parameters["__param1"]);
        }

        [Test]
        public void BinaryOps_Integers_GreaterThan()
        {
            var expression = LambdaExpression(doc => doc.SomeInt > 0);
            var parameters = new ScriptParameterDictionary();

            var result = ExpressionParser.CreateJsScriptFromExpression(expression, parameters);

            Assert.AreEqual(
                "(this.SomeInt > args.__param1)",
                result);

            Assert.AreEqual(1, parameters.Count);
            Assert.AreEqual(0, parameters["__param1"]);
        }

        [Test]
        public void BinaryOps_Integers_GreaterThanOrEqualTo()
        {
            var expression = LambdaExpression(doc => doc.SomeInt >= 0);
            var parameters = new ScriptParameterDictionary();

            var result = ExpressionParser.CreateJsScriptFromExpression(expression, parameters);

            Assert.AreEqual(
                "(this.SomeInt >= args.__param1)",
                result);

            Assert.AreEqual(1, parameters.Count);
            Assert.AreEqual(0, parameters["__param1"]);
        }

        [Test]
        public void BinaryOps_Integers_EqualTo()
        {
            var expression = LambdaExpression(doc => doc.SomeInt == 0);
            var parameters = new ScriptParameterDictionary();

            var result = ExpressionParser.CreateJsScriptFromExpression(expression, parameters);

            Assert.AreEqual(
                "(this.SomeInt == args.__param1)",
                result);

            Assert.AreEqual(1, parameters.Count);
            Assert.AreEqual(0, parameters["__param1"]);
        }

        [Test]
        public void BinaryOps_Integers_NotEqualTo()
        {
            var expression = LambdaExpression(doc => doc.SomeInt != 0);
            var parameters = new ScriptParameterDictionary();

            var result = ExpressionParser.CreateJsScriptFromExpression(expression, parameters);

            Assert.AreEqual(
                "(this.SomeInt != args.__param1)",
                result);

            Assert.AreEqual(1, parameters.Count);
            Assert.AreEqual(0, parameters["__param1"]);
        }

        private static LambdaExpression LambdaExpression<TProperty>(Expression<Func<TestDocument, TProperty>> expression)
        {
            // Convenience method for not having to explicitly write
            // ((Expression<Func<TestDocument, int[]>>)(doc => doc.Array))-like things everywhere.

            return expression;
        }
    }
}