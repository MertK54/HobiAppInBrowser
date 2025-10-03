using Backend.Hubs;
using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);

// SignalR ve CORS servislerini ekliyoruz
builder.Services.AddSignalR();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .SetIsOriginAllowed(_ => true) // tüm originlere izin
              .AllowCredentials();
    });
});

var app = builder.Build();

app.UseCors();
app.UseStaticFiles(); // 👉 wwwroot/index.html çalışsın

// Hub endpoint
app.MapHub<VideoChatHub>("/videochat");

// Önemli: 0.0.0.0:5000 dinle
app.Run("http://0.0.0.0:5000");
