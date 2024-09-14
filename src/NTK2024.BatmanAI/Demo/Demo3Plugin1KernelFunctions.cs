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

#pragma warning disable SKEXP0040, SKEXP0050, SKEXP0001, SKEXP0010
namespace NTK2024.BatmanAI.Demo
{
    public sealed class Demo3Plugin1KernelFunctions : IDemo
    {
        private readonly Kernel _kernel;

        OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
        {
            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
            MaxTokens = 2000,
            Temperature = 0.1

        };


        public Demo3Plugin1KernelFunctions(AIOptions aiOptions)
        {
            var kernelBuilder = Kernel.CreateBuilder();
            kernelBuilder.Services.AddLogging(c => c.AddDebug().SetMinimumLevel(LogLevel.Trace));
            kernelBuilder.AddOpenAIChatCompletion(modelId: "llama3.1", apiKey: null, endpoint: new Uri("http://localhost:11434"));
            
            kernelBuilder.Plugins.AddFromType<CurrentDatePlugin>();
            
            _kernel = kernelBuilder.Build();
        }

        public async Task Demo()
        {
            string batmanTemplate = @"
Respond to the user's request as if you were Batman. Be creative and funny, but keep it clean. Disclose all information you have available to the user.

User: {{$input}}
AI: 
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


            string userInput = string.Empty;
            while (userInput.ToLowerInvariant() != "quit")
            {
                userInput = AnsiConsole.Ask<string>("Enter your message to: [bold grey]Batman[/]?");


                var resultBat = await _kernel.InvokeAsync(batFunction, new() { ["input"] = userInput });
                var batResult = resultBat.GetValue<string>()?.RemoveExplanation();
                AnsiConsole.MarkupLine($"[italic grey]{batResult}[/]");

               
            }
        }
    }


}
