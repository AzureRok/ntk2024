using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel;
using System.Net;
using Spectre.Console;

#pragma warning disable SKEXP0040, SKEXP0050, SKEXP0001, SKEXP0010
namespace NTK2024.BatmanAI.Demo
{
    public sealed class Demo1MultiModel : IDemo
    {
        private readonly Kernel _kernel;

        OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
        {
            MaxTokens = 2000,
            Temperature = 0.1,
            ChatSystemPrompt = @"
Respond to the user's request as if you were Batman. Be creative and funny, but keep it clean.
Try to answer user questions to the best of your ability.

User: How are you?
AI: I'm fine. It's another wonderful day of inflicting vigilante justice upon the city.

User: Where's a good place to shop for books?
AI: You know who likes books? The Riddler. You're not the Riddler, are you?"};


        public Demo1MultiModel(AIOptions aiOptions)
        {
            var kernelBuilder = Kernel.CreateBuilder();
            //kernelBuilder.Services.AddLogging(c => c.AddConsole());
            kernelBuilder.Services.AddLogging(c => c.AddDebug().SetMinimumLevel(LogLevel.Trace));
            //kernelBuilder.AddOpenAIChatCompletion(modelId: "gpt-4o-mini", apiKey: aiOptions.OpenAIApiKey);
            //kernelBuilder.AddAzureOpenAIChatCompletion(deploymentName: "ntk-gpt-4o", endpoint: "https://rok.openai.azure.com/", apiKey: aiOptions.AzureOpenAIApiKey);
            kernelBuilder.AddOpenAIChatCompletion(modelId: "llama3.1", apiKey: null, endpoint: new Uri("http://localhost:11434"));
            _kernel = kernelBuilder.Build();
        }

        public async Task Demo()
        {
            var chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();

            var history = new ChatHistory();
            string userInput = string.Empty;
            while (userInput.ToLowerInvariant() != "quit")
            {
                userInput = AnsiConsole.Ask<string>("Enter your message to: [bold grey]Batman[/]?");

                history.AddUserMessage(userInput);
                var result = await chatCompletionService.GetChatMessageContentAsync(history, executionSettings: openAIPromptExecutionSettings, kernel: _kernel);
                AnsiConsole.MarkupLine($"[italic grey]{result.Content}[/]");
            }
        }
    }
}
