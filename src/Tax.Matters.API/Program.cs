using Microsoft.AspNetCore.Authorization;
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
builder.Services.AddSwaggerGen();

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
