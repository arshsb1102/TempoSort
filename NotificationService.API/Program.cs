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
using NotificationService.API.Infrastructure;
using NotificationService.DataAccess.Repositories;

var builder = WebApplication.CreateBuilder(args);
// Needed for Railway dynamic port
builder.WebHost.ConfigureKestrel(options =>
{
    var envPort = Environment.GetEnvironmentVariable("PORT");

    if (!string.IsNullOrEmpty(envPort))
    {
        // Railway or cloud env
        options.ListenAnyIP(int.Parse(envPort));
    }
    else
    {
        // Local dev (optional — change ports as needed)
        options.ListenLocalhost(5149); // HTTP
        options.ListenLocalhost(7177, listenOptions => listenOptions.UseHttps()); // HTTPS
    }
});
// Add services to the container.

builder.Services.AddControllers();
// Quartz config
var quartzConnStr = builder.Configuration.GetConnectionString("QuartzDb")
    ?? throw new InvalidOperationException("Missing 'QuartzDb' connection string in configuration.");
builder.Services.AddSingleton<IJobFactory, ScopedJobFactory>();
builder.Services.AddQuartz(q =>
{
    q.UsePersistentStore(store =>
    {
        store.UseProperties = true;
        store.UsePostgres(postgres =>
        {
            postgres.ConnectionString = quartzConnStr;
        });
        store.UseNewtonsoftJsonSerializer();
    });
    q.AddJob<DigestEmailJob>(job => job
        .WithIdentity("DigestEmailJob", "EmailJobs")
        .StoreDurably() // 👈 this is important
    );

    q.AddTrigger(trigger => trigger
        .ForJob("DigestEmailJob", "EmailJobs")
        .WithIdentity("DigestEmailTrigger", "EmailJobs")
        .WithCronSchedule("0 0/30 * * * ?")
        .WithDescription("Runs every 30 mins to send digest emails")
    );
});
builder.Services.AddQuartzHostedService(opt => opt.WaitForJobsToComplete = true);


//Add interfaces
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.AddTransient<ISmtpService, SmtpService>();
builder.Services.AddScoped<IConnectionFactory, ConnectionFactory>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddSingleton<EmailTemplateRenderer>();
builder.Services.AddScoped<IWelcomeEmailScheduler, WelcomeEmailScheduler>();
builder.Services.AddTransient<WelcomeEmailJob>();
builder.Services.AddTransient<DigestEmailJob>();



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
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
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
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
