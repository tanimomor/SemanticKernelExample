using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using DotNetEnv;

namespace Tasks
{
    public class Pluggin
    {
        public static async Task RunAsync()
        {
            // Load environment variables from .env file
            Env.Load();

            var modelId = "gpt-4o-mini";
            var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

            if (string.IsNullOrEmpty(apiKey))
            {
                Console.WriteLine("Error: OPENAI_API_KEY environment variable not set. Please add it to your .env file.");
            }

            var builder = Kernel.CreateBuilder().AddOpenAIChatCompletion(modelId, apiKey);

            var kernel = builder.Build();

            var funPluginDirectoryPath = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "plugins", "FunPlugin");

            var funPluginFunctions = kernel.ImportPluginFromPromptDirectory(funPluginDirectoryPath);

            var arguments = new KernelArguments() { ["input"] = "I'm a software developer. What's my favorite language?", ["style"] = "funny" };

            Console.WriteLine($@"=========================================
Arguments: 
  input: {arguments["input"]}
  style: {arguments["style"]}
=========================================");

            try
            {
                var result = await kernel.InvokeAsync(funPluginFunctions["Joke"], arguments);
                Console.WriteLine($"Result: {result}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }
    }
}
