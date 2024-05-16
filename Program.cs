using Amazon;
using Amazon.SecretsManager;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Singer.Application;
using Singer.Domain;
using Singer.Infrastructure;
using Singer.Interfaces;
using Singer.Utilities.Certificate;
using Singer.Utilities.Utils;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var secretsManagerConfig = new AmazonSecretsManagerConfig
{
    RegionEndpoint = RegionEndpoint.GetBySystemName("us-west-2")
};

builder.Services.AddSingleton<CertificateUtils>();

builder.Services.AddSingleton<AmazonSecretsManagerClient>(sp => new AmazonSecretsManagerClient(secretsManagerConfig));

builder.Services.AddTransient<IUserApplication, UserApplication>();

builder.Services.AddTransient<ISignerApplication, SignerApplication>();

builder.Services.AddTransient<IDWApplication, DWApplication>();

builder.Services.AddTransient<IDocumentApplication, DocumentApplication>();

builder.Services.AddTransient<IMessage, EmailApplication>();

builder.Services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

builder.Services.Configure<GmailSettings>(builder.Configuration.GetSection("Email"));

var configuration = builder.Configuration;

builder.Services.AddDbContext<SignerDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

var secret = builder.Configuration["Jwt:Key"];

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
        };
    });

builder.Services.AddSingleton<JwtService>(new JwtService(secret));

builder.Services.AddSingleton<PasswordHashService>();

builder.Services.AddScoped<JwtAuthorizationFilter>();

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.UseCors(builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();
});

app.MapControllers();

app.Run();
