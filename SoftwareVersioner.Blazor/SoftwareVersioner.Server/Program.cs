using SoftwareVersioner;
using SoftwareVersioner.Core.Interfaces;
using SoftwareVersioner.Server.Components;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<ISoftwareManager>(x => new SoftwareManager());

builder.Services.AddControllers();

builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.MapControllers();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(SoftwareVersioner.Client._Imports).Assembly);

app.Run();
