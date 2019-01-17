using System;
using System.Linq.Expressions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Sandbox.Documents;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Utilitites;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Sandbox
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var dummyChangeId = 1000;

            PrintCondition(doc => doc.Id != null);
            PrintCondition(doc => doc.Id != "");
            PrintCondition(doc => doc.Id != "a");
            PrintCondition(doc => doc.LastKnownChangeId > 10);
            PrintCondition(doc => doc.LastKnownChangeId == 0 ? true : doc.LastKnownChangeId > dummyChangeId);
            PrintCondition(doc => doc.LastKnownChangeId == 0 ? true : doc.LastKnownChangeId > dummyChangeId + 1);

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        private static void PrintCondition(Expression<Func<TestDocument, bool>> expression)
        {
            var originalColor = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Magenta;

            Console.WriteLine("***********************************************************************************");
            Console.WriteLine("C# Expression:");
            Console.Write("\t");
            Console.WriteLine(expression.ToString());
            Console.WriteLine();

            var parameters = new ScriptParameterDictionary();
            var result = ExpressionParser.CreateJsScriptFromExpression(expression, parameters);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Created JavaScript expression:");
            Console.Write("\t");
            Console.WriteLine(result);
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("Parameters:");
            foreach (var parameter in parameters)
            {
                Console.Write("\t");

                var value = parameter.Value;

                value = value == null
                    ? "null"
                    : (value is string str && str == string.Empty ? "(empty string)" : value);

                Console.WriteLine($"{parameter.Key}: {value}");
            }
            Console.WriteLine();

            Console.ForegroundColor = originalColor;
            Console.WriteLine();
        }
    }
}