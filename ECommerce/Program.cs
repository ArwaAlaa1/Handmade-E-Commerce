
using ECommerce.Core;
using ECommerce.DashBoard.Data;
using ECommerce.Repository.DbInitializer;
using ECommerce.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ECommerce.Core.Repository.Contract;
using ECommerce.Repository.Repositories;
using ECommerce.Core.Services.Contract;
using ECommerce.Services;
using ECommerce.Extentions;
using Microsoft.OpenApi.Models;
using ECommerce.Core.Services.Contract.SendEmail;
using StackExchange.Redis;
using ECommerce.Controllers;
using ECommerce.Helper;
using Microsoft.Extensions.DependencyInjection;
namespace ECommerce
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            
            //Sql Server Connection
            builder.Services.AddDbContext<ECommerceDbContext>(options =>
               options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")),
               ServiceLifetime.Transient);

            //Redis Connection
            builder.Services.AddSingleton<IConnectionMultiplexer>((provider) =>
            {
                var connection=builder.Configuration.GetConnectionString("Redis");
                return ConnectionMultiplexer.Connect(connection);
            });

            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IEmailProvider, EmailProvider>();

            builder.Services.AddScoped<ICartRepository, CartRepository>();
           

            builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
            builder.Services.AddScoped<IFavoriteRepository, FavoriteRepository>();

            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IOrderService,OrderService>();
            builder.Services.AddScoped<IAddressRepository, AddressRepository>();
            builder.Services.AddTransient<IReviewRepository, ReviewRepository>();

            builder.Services.AddAutoMapper(typeof(MappingProfiles));


            builder.Services.AddIdentityServices(builder.Configuration);
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                               .AllowAnyMethod()
                               .AllowAnyHeader();
                    });
            });

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            //builder.Services.AddOpenApi();
            builder.Services.AddEndpointsApiExplorer(); // Required for Swagger

            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Hand-made E-Commerce", Version = "v1" });
            });
            builder.Services.AddSwaggerGen(swagger =>
            {
                //This is to generate the Default UI of Swagger Documentation    
                swagger.SwaggerDoc("v2", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "ASP.NET 9 Web API",
                    Description = "Hand-made E-Commerce"
                });

                // To Enable authorization using Swagger (JWT)    
                swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9\"",
                });
                swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                    new string[] {}
                    }
                });
            });


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                //app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Hand-made E-Commerce v1"));
            }
            //app.UseSwagger();
            //app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Hand-made E-Commerce v1"));
            app.UseStaticFiles();
            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
