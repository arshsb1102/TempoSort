using NotificationService.Business.Interfaces;
using NotificationService.Business;
using NotificationService.DataAccess.Interfaces;
using NotificationService.DataAccess;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using Npgsql;
using Quartz;
using NotificationService.Business.Jobs;
using Quartz.Spi;
using NotificationService.Models.SMTP;
using NotificationService.DataAccess.SmtpService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Quartz config
var quartzConnStr = builder.Configuration.GetConnectionString("QuartzDb")
    ?? throw new InvalidOperationException("Missing 'QuartzDb' connection string in configuration.");

builder.Services.AddQuartz(q =>
{
    q.UseJobFactory<JobFactory>();

    q.UsePersistentStore(store =>
    {
        store.UseProperties = true;
        store.UsePostgres(postgres =>
        {
            postgres.ConnectionString = quartzConnStr;
        });
        store.UseNewtonsoftJsonSerializer();
    });

    var jobKey = new JobKey("EmailJob");
    q.AddJob<EmailJob>(opts =>
     opts.WithIdentity("EmailJob").StoreDurably());
});
builder.Services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);
builder.Services.AddTransient<EmailJob>();


//Add interfaces
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.AddTransient<ISmtpService, SmtpService>();
builder.Services.AddScoped<IConnectionFactory, ConnectionFactory>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<INotificationHelper, NotificationHelper>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddSingleton<IJobFactory, JobFactory>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddSingleton<EmailJob>();
builder.Services.AddSingleton<EmailTemplateRenderer>();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Notification API", Version = "v1" });

    // JWT Bearer Auth Setup
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token (no need to include 'Bearer')"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement {
    {
        new OpenApiSecurityScheme {
            Reference = new OpenApiReference {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        },
        new string[] {}
    }});
});
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings["Issuer"],

            ValidateAudience = true,
            ValidAudience = jwtSettings["Audience"],

            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["Key"]!))
        };
    });

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
app.MapControllers();

app.Run();
