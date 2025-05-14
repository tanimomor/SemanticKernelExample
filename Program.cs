// Import packages
using System.Runtime.CompilerServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using DotNetEnv;

// Load environment variables from .env file
Env.Load();

// Populate values from your OpenAI deployment
var modelId = "gpt-4o-mini";
var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");

// Check if API key is missing
if (string.IsNullOrEmpty(apiKey))
{
    Console.WriteLine("Error: OPENAI_API_KEY environment variable not set. Please add it to your .env file.");
    return;
}

var builder = Kernel.CreateBuilder().AddOpenAIChatCompletion(modelId, apiKey);

// Create a kernel with Azure OpenAI chat completizon
var kernel = builder.Build();

var prompt = @"{{ $input }}

One line TLDR with the fewest words.";

var summarize = kernel.CreateFunctionFromPrompt
(
    prompt,
    executionSettings: new OpenAIPromptExecutionSettings
    {
        MaxTokens = 100
    },
    functionName: "summarize"
);


string text = @"
    Far far away, behind the word mountains, far from the countries Vokalia and Consonantia, there live the blind texts. Separated they live in Bookmarksgrove right at the coast of the Semantics, a large language ocean. A small river named Duden flows by their place and supplies it with the necessary regelialia. It is a paradisematic country, in which roasted parts of sentences fly into your mouth. Even the all-powerful Pointing has no control about the blind texts it is an almost unorthographic life One day however a small line of blind text by the name of Lorem Ipsum decided to leave for the far World of Grammar. The Big Oxmox advised her not to do so, because there were thousands of bad Commas, wild Question Marks and devious Semikoli, but the Little Blind Text didn't listen. She packed her seven versalia, put her initial into the belt and made herself on the way. When she reached the first hills of the Italic Mountains, she had a last view back on the skyline of her hometown Bookmarksgrove, the headline of Alphabet Village and the subline of her own road, the Line Lane. Pityful a rethoric question ran over her cheek, then 
";

var output = await kernel.InvokeAsync(summarize, new() { ["input"] = text });

Console.WriteLine(output);