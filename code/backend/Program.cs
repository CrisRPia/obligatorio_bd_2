using SwaggerThemes;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapOpenApi();
app.UseSwaggerUI(Theme.Monokai);
app.MapSwagger();

app.MapGet("/", () => "Hello World!");
app.MapGet("/test/{sometext}", (string sometext) => {
    return sometext;
});

app.Run();
