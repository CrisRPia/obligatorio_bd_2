using System.Text.Json.Serialization;
using AspNetCore.Swagger.Themes;
using backend.src.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SwaggerThemes;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        Description =
            "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
    };

    options.AddSecurityDefinition("Bearer", jwtSecurityScheme);

    options.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer",
                    },
                },
                []
            },
        }
    );

    options.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();
});

builder
    .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var helper = new JwtService(builder.Configuration);
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = helper.Key,
            ValidAudiences = [helper.Audience],
            ValidIssuers = [helper.Issuer],
        };
    });

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
        options.OAuthUseBasicAuthenticationWithAccessCodeGrant();
    }
);

app.MapControllers();

app.Run();
