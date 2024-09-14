using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NTK2024.BatmanAI;
using NTK2024.BatmanAI.Demo;

var builder = Host.CreateDefaultBuilder(args);
builder.ConfigureHostConfiguration(app =>
{
    app.SetBasePath(Directory.GetCurrentDirectory());
    app.AddJsonFile("appsettings.json").AddUserSecrets<Program>();
});
builder.ConfigureLogging(config =>
{
    config.ClearProviders();
});
builder.ConfigureServices(
    (context, services) =>
    {
        services.Configure<AIOptions>(context.Configuration.GetSection(AIOptions.ElementName));
        var aiOptions = new AIOptions();
        context.Configuration.Bind(AIOptions.ElementName, aiOptions);
        services.AddSingleton(aiOptions);

        //services.AddTransient<IDemo, Demo1MultiModel>();
        //services.AddTransient<IDemo, Demo2Functions>();
        services.AddTransient<IDemo, Demo3Plugin1KernelFunctions>();
        //services.AddTransient<IDemo, Demo4Memory>();
        //services.AddTransient<IDemo, Demo5Connector>();
        //services.AddTransient<IDemo, Demo6Plugin2OpenAPI>();
    }
);

using IHost host = builder.Build();

using var scope = host.Services.CreateScope();
var services = scope.ServiceProvider;
var demo = services.GetRequiredService<IDemo>();

await demo.Demo();