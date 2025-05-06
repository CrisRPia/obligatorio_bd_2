var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapOpenApi();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("v1/swagger.json", "My API V1");
});
app.MapSwagger();

app.MapGet("/", () => "Hello World!");
app.MapGet("/test/{sometext}", (string sometext) => {
    return sometext;
});

app.Run();
