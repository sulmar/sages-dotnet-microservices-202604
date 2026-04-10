using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using BlazorApp;
using BlazorApp.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

 builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });



// dotnet add package Microsoft.Extensions.Http
builder.Services.AddHttpClient("ShopGateway", client => client.BaseAddress = new Uri("https://localhost:7000"));
builder.Services.AddHttpClient<IProductService, ApiProductService>(client => client.BaseAddress = new Uri("https://localhost:7000"));
builder.Services.AddScoped<ICartService>(sp =>
{
    var http = sp.GetRequiredService<IHttpClientFactory>().CreateClient("ShopGateway");
    return new ApiCartService(http);
});

await builder.Build().RunAsync();
