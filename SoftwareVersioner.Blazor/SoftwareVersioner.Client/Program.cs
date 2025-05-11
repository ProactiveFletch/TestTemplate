using SoftwareVersioner.Client.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddHttpClient("MyApiHttpClient", client =>
{
    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
});

builder.Services.AddScoped<IHttpSvc>(sp =>
{
    Console.WriteLine("Lol2");
    var hcfac = sp.GetRequiredService<IHttpClientFactory>();
    return new HttpSvc(hcfac.CreateClient("MyApiHttpClient"));
});
Console.WriteLine("Lol");

await builder.Build().RunAsync();
