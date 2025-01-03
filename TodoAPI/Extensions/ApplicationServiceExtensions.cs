using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TodoAPI.Data;
using TodoAPI.Repositories;
using TodoAPI.Services;

namespace TodoAPI.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config) {
            services.AddDbContext<AppDbContext>(opts => {
                opts.UseNpgsql(config.GetConnectionString("DB_CONNECTION_STRING"));
            });

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddScoped<IUserRepo, UserRepo>();
            services.AddScoped<ITodoRepo, TodoRepo>();
            services.AddScoped<ITodoItemRepo, TodoItemRepo>();

            return services;
        }
    }
}