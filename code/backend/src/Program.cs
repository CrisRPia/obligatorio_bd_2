using backend.src.Endpoints;
using SwaggerThemes;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(Theme.Monokai);
app.MapControllers();

Root.MapPing(app.MapGroup("/Minimal"));

app.Run();
