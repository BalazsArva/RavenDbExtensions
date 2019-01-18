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
            var dummyChangeId1 = 1000;
            var dummyChangeId2 = 2000;
            var arr = new[] { 100, 200, 300 };

            PrintCondition(doc => doc.LastKnownChangeId > 10);
            PrintCondition(doc => doc.LastKnownChangeId == 0 ? true : doc.LastKnownChangeId > dummyChangeId1);
            PrintCondition(doc => doc.LastKnownChangeId == 0 ? true : doc.LastKnownChangeId > dummyChangeId1 + 1);
            PrintCondition(doc => doc.LastKnownChangeId == 0 ? true : doc.LastKnownChangeId > arr[arr.Length - 1] + 1);
            PrintCondition(doc => doc.LastKnownChangeId == 0 ? true : (dummyChangeId1 < dummyChangeId2 ? true : false));
            PrintCondition(doc => doc.LastKnownChangeId > DateTime.UtcNow.Year);

            PrintCondition(doc => string.IsNullOrWhiteSpace(doc.Id) ? true : (doc.LastKnownChangeId < dummyChangeId1 ? true : false));
            PrintCondition(doc => doc.LastKnownChangeId.ToString() != "");

            PrintCondition(doc => doc.Id.Remove(1) != "");
            PrintCondition(doc => doc.Id.Remove(1, 2) != "");
            PrintCondition(doc => doc.Id.Remove(doc.Id.Length - 3, 2) != "");


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
            var script = ExpressionParser.CreateJsScriptFromExpression(expression, parameters);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Created JavaScript expression:");
            Console.Write("\t");
            Console.WriteLine(script);
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