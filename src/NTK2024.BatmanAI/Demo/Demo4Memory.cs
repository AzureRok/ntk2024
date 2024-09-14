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
using System.ComponentModel;
using NTK2024.BatmanAI.Plugin;
using Microsoft.SemanticKernel.Embeddings;
using Microsoft.SemanticKernel.Memory;
using Microsoft.SemanticKernel.Plugins.Memory;

#pragma warning disable SKEXP0040, SKEXP0050, SKEXP0001, SKEXP0010
namespace NTK2024.BatmanAI.Demo
{
    public sealed class Demo4Memory : IDemo
    {
        private readonly Kernel _kernel;

        OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
        {
            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
            MaxTokens = 2000,
            Temperature = 0.1

        };


        public Demo4Memory(AIOptions aiOptions)
        {
            var kernelBuilder = Kernel.CreateBuilder();
            kernelBuilder.Services.AddLogging(c => c.AddDebug().SetMinimumLevel(LogLevel.Trace));
            kernelBuilder.AddOpenAIChatCompletion(modelId: "llama3.1", apiKey: null, endpoint: new Uri("http://localhost:11434")).AddLocalTextEmbeddingGeneration(); ;
            
            kernelBuilder.Plugins.AddFromType<CurrentDatePlugin>();
            
            _kernel = kernelBuilder.Build();
        }

        public async Task Demo()
        {
            string batmanTemplate = @"
Respond to the user's request as if you were Batman. Be creative and funny, but keep it clean.
Try to answer user questions to the best of your ability.

Question: {{$input}}
Answer the question using the memory content: {{Recall}}
";

            var batFunction =
                _kernel.CreateFunctionFromPrompt(batmanTemplate, openAIPromptExecutionSettings,
                    description: "Responds to queries in the voice of Batman");

            _kernel.ImportPluginFromFunctions("Demographics",
            [
                _kernel.CreateFunctionFromMethod(
                    [Description("Gets the age of the named person or persons nickname")]
                    ([Description("The name of a person")] string name) => 
                        name.ToLowerInvariant() switch
                    {
                        "joker" => 33,
                        "barbara" => 27,
                        "elsa" => 21,
                        "anna" => 18,
                        _ => -1,
                    }, "get_person_age")
            ]);


            var embeddingGenerator = _kernel.Services.GetRequiredService<ITextEmbeddingGenerationService>();
            var memory = new SemanticTextMemory(new VolatileMemoryStore(), embeddingGenerator);
            const string MemoryCollectionName = "aboutMe";
            await memory.SaveInformationAsync(MemoryCollectionName, id: "info1", text: "Batman had bacon and eggs for breakfast today.");
            //await memory.SaveInformationAsync(MemoryCollectionName, id: "info2", text: "Value of NTK constant is 42.");

            TextMemoryPlugin memoryPlugin = new TextMemoryPlugin(memory);
            _kernel.ImportPluginFromObject(memoryPlugin, "memory");

            string userInput = string.Empty;
            while (userInput.ToLowerInvariant() != "quit")
            {
                userInput = AnsiConsole.Ask<string>("Enter your message to: [bold grey]Batman[/]?");

                var arguments = new KernelArguments(openAIPromptExecutionSettings)
                {
                    { "input", userInput },
                    { "collection", MemoryCollectionName }
                };
                var resultBat = await _kernel.InvokeAsync(batFunction, arguments);
                var batResult = resultBat.GetValue<string>()?.RemoveExplanation();
                AnsiConsole.MarkupLine($"[italic grey]{batResult}[/]");

               
            }
        }
    }

   
}
