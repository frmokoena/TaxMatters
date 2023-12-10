using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Tax.Matters.API.Core;
using Tax.Matters.API.Core.Security;
using Tax.Matters.Infrastructure;
using Tax.Matters.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ClientListOptions>(builder.Configuration.GetSection("AuthorizedClients"));

builder.Services.AddSingleton<IAuthorizationPolicyProvider, AuthorizationPolicyProviderAPICore>();
builder.Services.AddSingleton<IAuthorizationHandler, AuthorizationHandlerAPICore>();
builder.Services.AddBasicHeaderAuthentication();

builder.Services.AddHttpContextAccessor();
builder.Services.AddAPICoreServices();
builder.Services.AddDomainDbContext(
    builder.Configuration, connectionStringName: "AppDbContext");
builder.Services.AddCalculationRepository();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Tax Matters", Version = "v1" });
    c.AddSecurityDefinition("Basic", new OpenApiSecurityScheme
    {
        Description = "Basic auth added to authorization header",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = "basic",
        Type = SecuritySchemeType.Http
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference 
                { 
                    Type = ReferenceType.SecurityScheme, 
                    Id = "Basic" 
                }
            },
            new List<string>()
        }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Seed test data
//     Can be safely removed
await ContextDataSeeding.SeedContextDataAsync(app);

app.Run();
