using backend.src.Endpoints;
using SwaggerThemes;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapOpenApi();
app.UseSwaggerUI(Theme.Monokai);
app.MapSwagger();

app.MapGroup("/group").MapTestEndpoints();

app.Run();
