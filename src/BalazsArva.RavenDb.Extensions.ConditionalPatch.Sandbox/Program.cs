using System;
using System.Linq;
using System.Linq.Expressions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Expressions.Abstractions;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Factories;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Sandbox.Documents;
using BalazsArva.RavenDb.Extensions.ConditionalPatch.Utilitites;

namespace BalazsArva.RavenDb.Extensions.ConditionalPatch.Sandbox
{
    internal class Program
    {
        private static IExpressionProcessorPipeline expressionProcessorPipeline = ExpressionProcessorPipelineFactory.CreateExpressionProcessorPipeline();
        private static IPatchScriptBuilder patchScriptBuilder = PatchScriptBuilderFactory.CreatePatchScriptBuilder();
        private static IPatchScriptConditionBuilder patchScriptConditionBuilder = PatchScriptConditionBuilderFactory.CreatePatchScriptBodyBuilder();

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

            PrintCondition(doc => doc.Id.Length > 0);
            PrintCondition(doc => doc.RecordedChangeIds.Length > 0);
            PrintCondition(doc => doc.ReferenceIds.Count > 0);
            PrintCondition(doc => doc.InvolvedUsers.Count > 0);

            PrintCondition(doc => doc.Dictionary.Count > 0);
            PrintCondition(doc => doc.Dictionary.Keys.Count > 0);
            PrintCondition(doc => doc.Dictionary.Values.Count > 0);

            PrintCondition(doc => doc.InvolvedUsers.Any());
            PrintCondition(doc => doc.InvolvedUsers.Any(u => u == string.Empty));

            var script = patchScriptBuilder.CreateConditionalPatchScript(
                new PropertyUpdateBatch<TestDocument>()
                    .Add(doc => doc.LastKnownChangeId, dummyChangeId2)
                    .Add(doc => doc.RecordedChangeIds, new[] { 1L, 2L, 3L })
                    .CreateBatch(),
                (TestDocument doc) => doc.LastKnownChangeId < dummyChangeId2,
                new ScriptParameterDictionary());

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
            var script = patchScriptConditionBuilder.CreateScriptCondition(expression, parameters);

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

                string value;
                if (parameter.Value is string || parameter.Value is char)
                {
                    value = '"' + parameter.Value.ToString().Replace("\"", "\\\"") + '"';
                }
                else if (parameter.Value == null)
                {
                    value = "null";
                }
                else
                {
                    value = parameter.Value.ToString();
                }

                Console.WriteLine($"{parameter.Key}: {value}");
            }
            Console.WriteLine();

            Console.ForegroundColor = originalColor;
            Console.WriteLine();
        }
    }
}