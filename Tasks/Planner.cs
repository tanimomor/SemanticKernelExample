using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Memory;
using DotNetEnv;
using Microsoft.SemanticKernel.Plugins.Memory;
using Microsoft.SemanticKernel.Planning.Handlebars;

namespace Tasks
{
    public class Planner
    {
        public static async Task RunAsync()
        {
            Env.Load();

            var modelId = "gpt-4o-mini";
            var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

            var kernel = Kernel.CreateBuilder().AddOpenAIChatCompletion(modelId, apiKey).Build();

            #pragma warning disable SKEXP0001, SKEXP0003, SKEXP0010, SKEXP0011, SKEXP0020, SKEXP0021, SKEXP0050, SKEXP0060
            var planner = new HandlebarsPlanner();
            #pragma warning restore SKEXP0001, SKEXP0003, SKEXP0010, SKEXP0011, SKEXP0020, SKEXP0021, SKEXP0050, SKEXP0060
            
            var funPluginDirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), "Plugins");
            kernel.ImportPluginFromPromptDirectory(Path.Combine(funPluginDirectoryPath, "SummarizePlugin"));
            kernel.ImportPluginFromPromptDirectory(Path.Combine(funPluginDirectoryPath, "WriterPlugin"));

            var ask = "Tomorrow is my birthday. Write a poem about it.";
            
            #pragma warning disable SKEXP0001, SKEXP0003, SKEXP0010, SKEXP0011, SKEXP0020, SKEXP0021, SKEXP0050, SKEXP0060
            var originalPlan = await planner.CreatePlanAsync(kernel, ask);
            #pragma warning restore SKEXP0001, SKEXP0003, SKEXP0010, SKEXP0011, SKEXP0020, SKEXP0021, SKEXP0050, SKEXP0060
        }
    }
}
