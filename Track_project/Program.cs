using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Track_project.Data;
using Track_project.Services; 
using System; // For Console

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Configure services to use Newtonsoft.Json
builder.Services.AddControllersWithViews()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
    });

// Add DbContext using PostgreSQL provider
builder.Services.AddDbContext<DemoContext2>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add CORS policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

// Register WebSocketManagerService as a singleton
builder.Services.AddSingleton<WebSocketManagerService>();

var app = builder.Build();

// Start WebSocket Server
var webSocketManagerService = app.Services.GetRequiredService<WebSocketManagerService>();
webSocketManagerService.Start("ws://0.0.0.0:8181"); 

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error"); // Use a generic error handler page
    app.UseHsts(); // HTTP Strict Transport Security Protocol
}

app.UseHttpsRedirection(); // Redirect HTTP requests to HTTPS
app.UseStaticFiles(); // For serving static files like images, css, and JavaScript

app.UseRouting(); // Adds routing capabilities

app.UseCors("AllowAllOrigins"); // Use the CORS policy

app.UseAuthorization(); // Adds authorization capabilities

// Map controller route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run(); // Run the application