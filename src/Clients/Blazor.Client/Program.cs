using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using BlazorApp;
using BlazorApp.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

 builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });



// dotnet add package Microsoft.Extensions.Http
builder.Services.AddHttpClient<IProductService, ApiProductService>(client => client.BaseAddress = new Uri("https://localhost:7126"));
builder.Services.AddHttpClient<ICartService, ApiCartService>(client => client.BaseAddress = new Uri("https://localhost:7286"));

await builder.Build().RunAsync();
