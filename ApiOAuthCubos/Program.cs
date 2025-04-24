using ApiOAuthCubos.Data;
using ApiOAuthCubos.Helpers;
using ApiOAuthCubos.Repositories;
using ApiOAuthCubos.Services;
using Azure.Security.KeyVault.Secrets;
using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAzureClients(factory =>
{
    factory.AddSecretClient(builder.Configuration.GetSection("KeyVault"));

});
SecretClient secretClient = builder.Services.BuildServiceProvider().GetService<SecretClient>();

HelperCryptography.Initialize(builder.Configuration, secretClient);
builder.Services.AddTransient<HelperUserToken>();
builder.Services.AddHttpContextAccessor();

HelperActionServicesOAuth helper = new HelperActionServicesOAuth(builder.Configuration, secretClient);
builder.Services.AddSingleton<HelperActionServicesOAuth>(helper);
builder.Services.AddAuthentication(helper.GetAuthenticateSchema()).AddJwtBearer(helper.GetJwtBearerOptions());


KeyVaultSecret secret = await secretClient.GetSecretAsync("SqlAzure");
string connectionString = secret.Value;

builder.Services.AddDbContext<CubosContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddTransient<RepositoryCubos>();

KeyVaultSecret storageSecret = await secretClient.GetSecretAsync("Storage");
string storage = storageSecret.Value;
BlobServiceClient blobService = new BlobServiceClient(storage);
builder.Services.AddTransient<BlobServiceClient>(x => blobService);
builder.Services.AddTransient<ServiceStorageBlob>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}

app.MapOpenApi();
app.MapScalarApiReference();
app.MapGet("/", context =>
{
    context.Response.Redirect("/scalar");
    return Task.CompletedTask;
});

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
