using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Movies.Core.CustomEntities;
using Movies.Core.Interfaces;
using Movies.Core.Services;
using Movies.Infrastructure.Data;
using Movies.Infrastructure.Filters;
using Movies.Infrastructure.Mappings;
using Movies.Infrastructure.Repositories;
using Movies.Infrastructure.Validators;

namespace Movies.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            // Configuraci�n base
            builder.Configuration.Sources.Clear();
            builder.Configuration
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

            // User Secrets solo en desarrollo
            if (builder.Environment.IsDevelopment())
            {
                builder.Configuration.AddUserSecrets<Program>();
                Console.WriteLine("User Secrets habilitados para desarrollo");
            }

            // Variables de entorno (para producci�n)
            builder.Configuration.AddEnvironmentVariables();


            // En produccion los secretos vendran en entornos globales
            #region Configurar la BD SqlServer
            var connectionString = builder.Configuration.GetConnectionString("ConnectionSqlServer");
            builder.Services.AddDbContext<MoviesContext>(options =>
                options.UseSqlServer(connectionString, sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,              // Máximo 5 reintentos
                        maxRetryDelay: TimeSpan.FromSeconds(10),  // Espera máxima entre reintentos
                        errorNumbersToAdd: null         // Puedes agregar códigos de error adicionales si quieres
                    );
                })
            );

            #endregion

            #region Configurar la BD MySql
            //var connectionString = builder.Configuration.GetConnectionString("ConnectionMySql");
            //builder.Services.AddDbContext<SocialMediaContext>(options =>
            //    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
            #endregion

            builder.Services.AddAutoMapper(typeof(MappingProfile));

            // Inyectar las dependencias
            builder.Services.AddScoped<IMovieRepository, MovieRepository>();
            builder.Services.AddScoped<IMovieServices, MovieServices>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IUserServices, UserServices>();
            builder.Services.AddScoped<IActorRepository, ActorRepository>();
            builder.Services.AddScoped<IActorServices, ActorServices>();
            builder.Services.AddScoped<ICommentRepository, CommentRepository>();
            builder.Services.AddScoped<ICommentServices, CommentServices>();
            builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
            builder.Services.AddScoped<IReviewServices, ReviewServices>();
            builder.Services.AddScoped<ISecurityService, SecurityService>();
            builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddSingleton<IDBConnectionFactory, DbConnectionFactory>();
            builder.Services.AddScoped<IDapperContext, DapperContext>();
            builder.Services.AddSingleton<IPasswordService, PasswordService>();

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme);
            // Add services to the container.
            builder.Services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            }).ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            builder.Services.AddControllers(options =>
            {
                options.Filters.Add<GlobalExceptionFilter>();
            }).AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            }).ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            builder.Services.Configure<PasswordOptions>
            (builder.Configuration.GetSection("PasswordOptions"));


            //Validaciones
            builder.Services.AddControllers(options =>
            {
                options.Filters.Add<ValidationFilter>();
            });


            //Configuracion
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new()
                {
                    Title = "Backend MoviesReviews API",
                    Version = "v1",
                    Description = "Documentacion de la API de MoviesReview - NET 9",
                    Contact = new()
                    {
                        Name = "Equipo de Desarrollo UCB",
                        Email = "desarrollo@ucb.edu.bo"
                    }
                });

                // Esto mantiene los comentarios XML
                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Escribe: Bearer {tu token JWT}"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
            });




            builder.Services.AddApiVersioning(options =>
            {
                
                options.ReportApiVersions = true;

               
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new ApiVersion(1, 0);

                options.ApiVersionReader = ApiVersionReader.Combine(
                    new UrlSegmentApiVersionReader(),       
                    new HeaderApiVersionReader("x-api-version"), 
                    new QueryStringApiVersionReader("api-version") 
                );
            });

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme =
                    JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme =
                    JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Authentication:Issuer"],
                        ValidAudience = builder.Configuration["Authentication:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(
                            System.Text.Encoding.UTF8.GetBytes(
                                builder.Configuration["Authentication:SecretKey"]
                            )
                        )
                    };
            });

           


            // FluentValidation
            builder.Services.AddValidatorsFromAssemblyContaining<ActorDtoValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<CommentDtoValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<ReviewDtoValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<UserDtoValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<MovieDtoValidator>();


            builder.Services.AddValidatorsFromAssemblyContaining<GetByIdRequestValidator>();

            // Services


            builder.Services.Configure<PasswordOptions>
                (builder.Configuration.GetSection("PasswordOptions"));

            // Configuraci�n base
            builder.Configuration.Sources.Clear();
            builder.Configuration
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables(); // �ESTO ES CLAVE PARA AZURE




            builder.Services.AddScoped<IValidationService, ValidationService>();

            var app = builder.Build();

            //Usar Swagger

            //Usar Swagger
            //if (app.Environment.IsDevelopment())
            //{
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Backend MoviesReviews API v1");
                options.RoutePrefix = string.Empty;
            });
            //}
           


            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

           

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
