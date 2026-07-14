using Banking.Api.Middleware;
using Banking.Application.Extensions;
using Banking.Application.Security;
using Banking.Application.Security.Interfaces;
using Banking.Infrastructure.Data;
using Banking.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions
            .Converters.Add(
                new JsonStringEnumConverter());
    });

builder.Services.AddApplication();

builder.Services.AddAuthentication(
    JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer =
                    builder.Configuration["Jwt:Issuer"],

                ValidAudience =
                    builder.Configuration["Jwt:Audience"],

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            builder.Configuration["Jwt:Key"]!
                        ))
            };
    });

builder.Services.AddAuthorization();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("Jwt"));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc(
        "v1",
        new OpenApiInfo
        {
            Title = "Banking API - Banco Digital Nova",
            Version = "v1",
            Description =
            """
            API REST bancaria desarrollada con .NET 8.

            Permite gestionar:
            - Usuarios y autenticación JWT.
            - Clientes.
            - Cuentas bancarias.
            - Transferencias.
            - Movimientos y auditoría mediante TraceId.

            Arquitectura basada en Clean Architecture.
            """,
            Contact = new OpenApiContact
            {
                Name = "Banco Digital Nova"
            }
        });


    var xmlFile =
        $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";


    var xmlPath = Path.Combine(
        AppContext.BaseDirectory,
        xmlFile);


    options.IncludeXmlComments(xmlPath);


    options.AddSecurityDefinition(
    "Bearer",
    new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingrese: Bearer {token}"
    });


    options.AddSecurityRequirement(document =>
    {
        return new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference(
                "Bearer",
                document),
            new List<string>()
        }
    };
    });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider
        .GetRequiredService<BankingDbContext>();

    var passwordHasher = scope.ServiceProvider
        .GetRequiredService<IPasswordHasher>();

    await DatabaseSeeder.SeedAsync(
        context,
        passwordHasher);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
