using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.BearerToken;
using SwaggerThemes;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// TODO: ADD JWT SUPPORT (BY ROLE)
builder
    .Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter()
        );
    });

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(Theme.Monokai);
app.MapControllers();

app.Run();
