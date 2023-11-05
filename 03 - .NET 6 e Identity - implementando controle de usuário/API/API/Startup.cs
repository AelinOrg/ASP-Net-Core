using System.Text;
using API.Attributes;
using API.Data;
using API.Data.Enums;
using API.Helpers;
using API.Models;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace API;

public class Startup
{
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        // Configuração do banco de dados
        services.AddDbContext<ApiDbContext>(options =>
        {
            options
                .UseSqlServer(Configuration.GetConnectionString("DefaultConnection"))
                .UseSnakeCaseNamingConvention();
        });

        // Configuração do Identity
        services
            .AddIdentity<User, IdentityRole>(options =>
            {
                // Autenticação
                options.Password.RequireDigit = false;
                options.Password.RequireUppercase = false;

                // Bloqueio
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);

                // Usuário
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ApiDbContext>()
            .AddDefaultTokenProviders();

        // Configuração do JWT
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ClockSkew = TimeSpan.Zero,
                    IssuerSigningKey = new SymmetricSecurityKey
                    (
                        Encoding.ASCII.GetBytes(Configuration["SigningKey"])
                    )
                };
            });

        // Configuração de autorização
        services.AddAuthorization();

        // Configuração dos serviços da aplicação
        services.AddControllers();

        // Injeção de serviços
        IEnumerable<Type> serviceTypes = AssemblyHelper.GetTypesWithAttribute<ServiceAttribute>();

        foreach (Type serviceType in serviceTypes)
        {
            services.AddScoped(serviceType);
        }

        // Configuração do CORS
        services.AddCors();

        // Configuração do Swagger
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        // Configuração do AutoMapper
        services.AddAutoMapper(typeof(Startup));

        // Configuração do FluentValidation
        services.AddFluentValidationAutoValidation();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceCollection services)
    {
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        CreateRoles(services);
    }

    private static void CreateRoles(IServiceCollection services)
    {
        using (IServiceScope scope = services.BuildServiceProvider().CreateScope())
        {
            IServiceProvider serviceProvider = scope.ServiceProvider;
            RoleManager<IdentityRole> roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string[] roles = Enum.GetNames(typeof(UserRoleEnum));
            List<string> databaseRoles = roleManager.Roles.Select(role => role.Name).ToList();

            foreach (string missingRole in roles.Except(databaseRoles))
            {
                roleManager.CreateAsync(new IdentityRole(missingRole)).Wait();
            }
        }
    }
}
