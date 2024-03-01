using Microsoft.AspNetCore.Hosting;
using Reenbit.WebApp.Components;
using Reenbit.WebApp.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddUserSecrets<Program>();

Environment.SetEnvironmentVariable("CONNECTION_STRING", builder.Configuration["CONNECTION_STRING"]);
Environment.SetEnvironmentVariable("CONTAINER_NAME", builder.Configuration["CONTAINER_NAME"]);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddScoped<IFileService, FileService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
