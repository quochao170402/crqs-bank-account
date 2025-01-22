using System.IO.Compression;
using System.Reflection;
using CQRS.Read.Consumers;
using CQRS.Read.Models;
using CQRS.Read.Services;
using Events;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.ResponseCompression;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMediatR(Assembly.GetExecutingAssembly());
// Add response compression services
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true; // Enable compression for HTTPS
    options.Providers.Add<GzipCompressionProvider>();
});

// Optional: Configure Gzip compression level
builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = CompressionLevel.Fastest; // Choose compression level
});

#region MongoDB Configuration

var mongoClient = new MongoClient("mongodb://localhost:27017");
var mongoDatabase = mongoClient.GetDatabase("CQRS");
builder.Services.AddSingleton(mongoDatabase);
builder.Services.AddScoped<IMongoCollection<Account>>(_ =>
    mongoDatabase.GetCollection<Account>("Accounts"));

#endregion

builder.Services.AddScoped<IAccountService, AccountService>();

// Add MassTransit
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<AccountClosedConsumer>();
    x.AddConsumer<AccountCreatedConsumer>();
    x.AddConsumer<DepositedConsumer>();
    x.AddConsumer<WithdrewConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.UseMessageRetry(r => r.Immediate(5));

        cfg.ReceiveEndpoint(nameof(BankAccountCreated), e => { e.ConfigureConsumer<AccountCreatedConsumer>(context); });
        cfg.ReceiveEndpoint(nameof(BankAccountClosed), e => { e.ConfigureConsumer<AccountClosedConsumer>(context); });
        cfg.ReceiveEndpoint(nameof(Deposited), e => { e.ConfigureConsumer<DepositedConsumer>(context); });
        cfg.ReceiveEndpoint(nameof(Withdrew), e => { e.ConfigureConsumer<WithdrewConsumer>(context); });
    });
});

builder.Services.AddMassTransitHostedService();

var app = builder.Build();

// Enable response compression middleware
app.UseResponseCompression();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors(x => x
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
