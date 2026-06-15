
using FluentValidation;
using GoldenCrown.Application.Feauters.User.UserLogin;
using GoldenCrown.BackGroundServices;
using GoldenCrown.Database;
using GoldenCrown.DTOs.User;
using GoldenCrown.Infrastructure.RabbitMQ;
using GoldenCrown.MiddleWare;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace GoldenCrown
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found");

            builder.Services.AddDbContext<ApplicationDBContext>(options => 
                options.UseSqlServer(connectionString));

            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(UserLoginCommand).Assembly));

            builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMQ"));
            builder.Services.AddSingleton<IMessageProducer,RabbitMqMessageProducer>();

            builder.Services.AddProblemDetails();

            builder.Services.AddValidatorsFromAssemblyContaining<LoginRequest>();

            builder.Services.AddAutoMapper(_ => { }, typeof(Program).Assembly);

            builder.Services.AddHostedService<SessionCleanupService>();

            builder.Services.AddControllers();


            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.AddSwaggerGen();

            var app = builder.Build();


            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseExceptionHandler();

            app.UseStatusCodePages();

            app.UseHttpsRedirection();

            app.UseMiddleware<AuthorizationMiddleware>();


            app.MapControllers();

            MigrateDatabase(app);

            app.Run();
        }

        private static void MigrateDatabase(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
            db.Database.Migrate();
        }
    }
}
