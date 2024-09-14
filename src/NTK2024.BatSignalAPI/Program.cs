using Microsoft.AspNetCore.Rewrite;
using Microsoft.OpenApi.Models;
using NTK2024.BatSignalAPI;
using OpenAIPluginMiddleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSignalR();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Bat API", Version = "v1" });
});

builder.Services.AddAiPluginGen(options =>
{
    options.NameForHuman = "BatSignal API";
    options.NameForModel = "batsignalapi";
    options.LegalInfoUrl = "https://example.com/legal";
    options.ContactEmail = "noreply@example.com";
    options.LogoUrl = "https://example.com/logo.png";
    options.DescriptionForHuman = "Controls the Bat Signal";
    options.DescriptionForModel = "Plugin for controlling the bat signal. Use It whenever a users asks to turn on or off the bat signal or asks about its status.";
    options.ApiDefinition = new Api() { RelativeUrl = "/swagger/v1/swagger.yaml" };
}); // Not needed anymore but can still be useful if used with OpenAI

var app = builder.Build();

app.UseSwagger();
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerUI();
}
app.UseRewriter(new RewriteOptions().AddRedirect("^$", "index.html"));

app.UseHttpsRedirection();
app.UseAiPluginGen();

app.UseStaticFiles();

app.MapControllers();
app.MapHub<BatHub>("/batHub");

app.Run();
