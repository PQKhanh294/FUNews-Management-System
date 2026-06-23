using System.Text;
using System.Text.Json.Serialization;
using FUNewsManagementSystem.API.Infrastructure;
using FUNewsManagementSystem.Services.Auth;
using FUNewsManagementSystem.Services.Helpers;
using FUNewsManagementSystem.Services.Security;
using FUNewsManagementSystem.Services.Services;
using FUNewsManagementSystem.Services.Services.Interfaces;
using FUNewsManagementSystem.Services.Validators;
using FUNewsManagementSystem.BusinessObjects.Options;
using FUNewsManagementSystem.BusinessObjects.Security;
using FUNewsManagementSystem.DataAccess.Context;
using FUNewsManagementSystem.Repositories;
using FUNewsManagementSystem.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.Configure<AdminAccountOptions>(builder.Configuration.GetSection("AdminAccount"));
builder.Services.Configure<GoogleAuthOptions>(builder.Configuration.GetSection("GoogleAuth"));

var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>() ?? new JwtOptions();

builder.Services.AddDbContext<FUNewsManagementContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("FUNewsManagement")));

builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<INewsArticleRepository, NewsArticleRepository>();
builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<IAuthenticationStrategy, JwtPasswordAuthStrategy>();
builder.Services.AddScoped<IAuthenticationStrategy, GoogleAuthStrategy>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ITagService, TagService>();
builder.Services.AddScoped<INewsArticleService, NewsArticleService>();
builder.Services.AddScoped<INewsApprovalService, NewsApprovalService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<INotificationService, LogNotificationService>();
builder.Services.AddScoped<INewsArticleValidator, NewsArticleValidator>();
builder.Services.AddSingleton<ISeoMetadataBuilder, SeoMetadataBuilder>();
builder.Services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
builder.Services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

builder.Services
    .AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles)
    .AddOData(options => options
        .Select()
        .Filter()
        .OrderBy()
        .Count()
        .Expand()
        .SetMaxTop(100));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy.WithOrigins(builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>())
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(AuthorizationPolicies.AdminOnly, policy => policy.RequireRole(AuthorizationPolicies.AdminRoleName));
    options.AddPolicy(AuthorizationPolicies.StaffOnly, policy => policy.RequireRole(AuthorizationPolicies.StaffRoleName));
    options.AddPolicy(AuthorizationPolicies.AdminOrStaff, policy => policy.RequireRole(AuthorizationPolicies.AdminRoleName, AuthorizationPolicies.StaffRoleName));
});

var app = builder.Build();
await app.Services.EnsureSqlServerSchemaAsync();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("Frontend");
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

