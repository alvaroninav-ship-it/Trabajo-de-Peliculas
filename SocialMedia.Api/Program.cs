using FluentValidation;
using Microsoft.EntityFrameworkCore;
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

            #region Configurar la BD SqlServer
            var connectionString = builder.Configuration.GetConnectionString("ConnectionSqlServer");
            builder.Services.AddDbContext<MoviesContext>(options => options.UseSqlServer(connectionString));
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
            builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            // Add services to the container.
            builder.Services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            }).ConfigureApiBehaviorOptions(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            //Validaciones
            builder.Services.AddControllers(options =>
            {
                options.Filters.Add<ValidationFilter>();
            });

            // FluentValidation
            builder.Services.AddValidatorsFromAssemblyContaining<ActorDtoValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<CommentDtoValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<ReviewDtoValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<UserDtoValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<MovieDtoValidator>();


            builder.Services.AddValidatorsFromAssemblyContaining<GetByIdRequestValidator>();

            // Services
            builder.Services.AddScoped<IValidationService, ValidationService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
