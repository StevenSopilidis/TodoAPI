using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.EntityFrameworkCore;
using TodoAPI.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Runtime.CompilerServices;
using TodoAPI.Extensions;
using Xunit.Abstractions;

[assembly: InternalsVisibleTo("TodoApiTests")]

namespace TodoApiTests
{
    // integration test for user Api
    internal class TodoApiApplication : WebApplicationFactory<Program>
    {
        private static readonly Lazy<TodoApiApplication> _instance = new Lazy<TodoApiApplication>(() => {
            var app = new TodoApiApplication();
            app.ConfigureApp();
            return app;
        });
        public static TodoApiApplication Instance = _instance.Value;

        private void ConfigureApp()
        {   
            // override service registration for testing perpoces
            WithWebHostBuilder(builder => {
                builder.ConfigureTestServices(services => {
                    // add test database
                    services.RemoveAll(typeof(DbContextOptions<AppDbContext>)); // remove real database connection string

                    var configuration = new ConfigurationBuilder()
                        .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .Build();

                    services.AddApplicationServices(configuration);

                    var connectionString = configuration.GetConnectionString("TEST_DB_CONNECTION_STRING");

                    services.AddDbContext<AppDbContext>(opts => {
                        opts.UseNpgsql(connectionString);
                    });

                    // ensure deletion on database
                    var dbContext = services.BuildServiceProvider();
                    var scoped = dbContext.CreateScope();
                    var context = scoped.ServiceProvider.GetService<AppDbContext>();

                    context.Database.EnsureDeleted();
                    context.Database.Migrate();
                });
            });
        }

    }
}