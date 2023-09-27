using System.Reflection;
using System.Text;
using API.Authorization;
using API.Data;
using API.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Add services to the container.
var assembly = Assembly.GetExecutingAssembly();

var serviceTypes = assembly.GetTypes()
    .Where(type => type.IsClass && !type.IsAbstract && type.Name.EndsWith("Service"))
    .ToList();

Console.WriteLine(serviceTypes.Count);

foreach (var serviceType in serviceTypes)
{
    builder.Services.AddScoped(serviceType);
}

builder.Services.AddDbContext<ApiDbContext>(options =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)).UseSnakeCaseNamingConvention();
});

builder.Services
    .AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<ApiDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddSingleton<IAuthorizationHandler, AuthPolicyHandler>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAuth", policy => policy.AddRequirements(new AuthPolicy()));
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey
            (
                Encoding.ASCII.GetBytes(builder.Configuration["SigningKey"])
            ),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
