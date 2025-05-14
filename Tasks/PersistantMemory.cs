using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Memory;
using DotNetEnv;
using Microsoft.SemanticKernel.Plugins.Memory;

namespace Tasks
{
    public class PersistantMemory
    {
        public static async Task RunAsync()
        {
            // Load environment variables from .env file
            Env.Load();

            // Populate values from your OpenAI deployment
            var modelId = "gpt-4o-mini";
            var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

            var kernel = Kernel.CreateBuilder().AddOpenAIChatCompletion(modelId, apiKey).Build();

            var prompt = @"Chatbot can have a conversation with you on any topic.

            Information about me, from previous conversations:
            {{$fact}} {{recall $fact}}
            
            Chat:
            Bot: {{$history}}
            User Input: {{$userInput}}
            Chatbot:";

            #pragma warning disable SKEXP0001, SKEXP0003, SKEXP0010, SKEXP0011, SKEXP0020, SKEXP0021, SKEXP0050, SKEXP0060
            var memoryBuilder = new MemoryBuilder();

            // Check if memoryBuilder was properly created
            if (memoryBuilder == null)
            {
                Console.WriteLine("Error: Failed to create memory builder");
                return;
            }
            memoryBuilder.WithOpenAITextEmbeddingGeneration(
                "text-embedding-ada-002", apiKey
            );
            // Add memory store before building
            memoryBuilder?.WithMemoryStore(new VolatileMemoryStore());
            var memory = memoryBuilder.Build();
            #pragma warning restore SKEXP0050, SKEXP0052

            var chatFunction = kernel.CreateFunctionFromPrompt
            (
                prompt,
                executionSettings: new OpenAIPromptExecutionSettings
                {
                    MaxTokens = 100,
                    Temperature = 0.7,
                    TopP = 0.5
                }
            );

            var userInput = "";
            var history = "";

            const string MemoryCollectionName = "chatHistoryCollection";


            var arguments = new KernelArguments() 
            { 
                ["history"] = "", 
                ["fact"] = "My name is Tanim. I am a software developer."
                
            };
            #pragma warning disable SKEXP0001, SKEXP0003, SKEXP0010, SKEXP0011, SKEXP0020, SKEXP0021, SKEXP0050, SKEXP0052, SKEXP0060, SKEXP0100
            arguments[TextMemoryPlugin.CollectionParam] = MemoryCollectionName;
            arguments[TextMemoryPlugin.LimitParam] = 2;
            arguments[TextMemoryPlugin.RelevanceParam] = 0.8;

            kernel.ImportPluginFromObject(new TextMemoryPlugin(memory));
            #pragma warning restore SKEXP0001, SKEXP0003, SKEXP0010, SKEXP0011, SKEXP0020, SKEXP0021, SKEXP0050, SKEXP0052, SKEXP0060, SKEXP0100


            

            Console.WriteLine("Hi! I am a chatbot. How can I help you today?");

            Func<String, Task> Chat = async (input) =>
                {
                    arguments["userInput"] = input;


                    var answer = await chatFunction.InvokeAsync(kernel, arguments);

                    var result = $"\nUser: {input}\nAI: {answer}\n";

                    history += result;

                    arguments["history"] = history;

                    Console.WriteLine("Answer;"+answer.ToString()+"History:"+arguments["history"]);
                };

            while (true)
            {
                var readUserInput = Console.ReadLine();

                await Chat(readUserInput);
            }

        }
    }
}
