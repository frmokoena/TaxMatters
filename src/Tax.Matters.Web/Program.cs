using Tax.Matters.Client;
using Tax.Matters.Web.Core;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ClientOptions>(builder.Configuration.GetSection("Client"));


builder.Services.AddAPIClient();
builder.Services.AddWebCoreServices();
builder.Services.AddHttpContextAccessor();

builder.Services.AddRazorPages();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
