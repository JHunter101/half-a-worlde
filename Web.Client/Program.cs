using Core.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Web.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddScoped<IGameStateService, GameStateService>();
builder.Services.AddScoped<IGuessValidator, GuessValidator>();
builder.Services.AddScoped<IWordGenerator, WordGenerator>();
builder.Services.AddScoped<IWordRepository, WordRepository>();

await builder.Build().RunAsync();