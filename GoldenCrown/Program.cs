
using GoldenCrown.BackGroundServices;
using GoldenCrown.Database;
using GoldenCrown.MiddleWare;
using GoldenCrown.Services;
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

            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddScoped<IFinanceService, FinanceService>();

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

            app.UseHttpsRedirection();

            app.UseMiddleware<AuthorizationMiddleware>();


            app.MapControllers();

            app.Run();
        }
    }
}
