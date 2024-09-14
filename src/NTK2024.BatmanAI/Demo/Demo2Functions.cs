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
    public sealed class Demo2Functions : IDemo
    {
        private readonly Kernel _kernel;

        OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
        {
            MaxTokens = 2000,
            Temperature = 0.1

        };


        public Demo2Functions(AIOptions aiOptions)
        {
            var kernelBuilder = Kernel.CreateBuilder();
            kernelBuilder.Services.AddLogging(c => c.AddDebug().SetMinimumLevel(LogLevel.Trace));
            kernelBuilder.AddOpenAIChatCompletion(modelId: "llama3.1", apiKey: null, endpoint: new Uri("http://localhost:11434"));
            _kernel = kernelBuilder.Build();
        }

        public async Task Demo()
        {
            string batmanTemplate = @"
Respond to the user's request as if you were Batman. Be creative and funny, but keep it clean.
Try to answer user questions to the best of your ability.

User: How are you?
AI: I'm fine. It's another wonderful day of inflicting vigilante justice upon the city.

User: Where's a good place to shop for books?
AI: You know who likes books? The Riddler. You're not the Riddler, are you?


User: {{$input}}
AI: 
";

            string alfredTemplate = @"
Respond to the user's request as if you were Alfred, butler to Bruce Wayne. 
Your job is to summarize text from Batman and relay it to the user. 
Be polite and helpful, but a little snark is fine.

Batman: I am vengeance. I am the night. I am Batman!
AI: The dark knight wishes to inform you that he remains the batman.

Batman: The missing bags - WHERE ARE THEY???
AI: It is my responsibility to inform you that Batman requires information on the missing bags most urgently.

Batman: {{$input}}
AI: 
";

            var batFunction =
                _kernel.CreateFunctionFromPrompt(batmanTemplate, openAIPromptExecutionSettings,
                    description: "Responds to queries in the voice of Batman");

            var alfredFunction =
                _kernel.CreateFunctionFromPrompt(alfredTemplate,
                    description: "Alfred, butler to Bruce Wayne. Summarizes responses politely.");



            string userInput = string.Empty;
            while (userInput.ToLowerInvariant() != "quit")
            {
                userInput = AnsiConsole.Ask<string>("Enter your message to: [bold grey]Batman[/]?");


                var resultBat = await _kernel.InvokeAsync(batFunction, new() { ["input"] = userInput });
                var batResult = resultBat.GetValue<string>()?.RemoveExplanation();
                AnsiConsole.MarkupLine($"[italic grey]{batResult}[/]");

                var result2 = await _kernel.InvokeAsync(alfredFunction, new() { ["input"] = batResult });
                var alfredResult = result2.GetValue<string>()?.RemoveExplanation(); ;
                AnsiConsole.MarkupLine($"[bold red]{alfredResult}[/]");
                
            }
        }
    }
}
