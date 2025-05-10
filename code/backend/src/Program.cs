using System.Text.Json.Serialization;
using AspNetCore.Swagger.Themes;
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
app.UseSwaggerUI(
    ModernStyle.Dark,
    options =>
    {
        options.EnableAllAdvancedOptions();
        options.DocumentTitle = "Voting backend";
        options.EnablePersistAuthorization();
        options.EnableTryItOutByDefault();
        options.DefaultModelExpandDepth(10);
        options.DefaultModelsExpandDepth(10);
        options.EnableSwaggerDocumentUrlsEndpoint();
    }
);

app.MapControllers();

app.Run();
