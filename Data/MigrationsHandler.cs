using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TodoAPI.Data
{
    public class MigrationsHandler
    {
        public static void ApplyMigrations(WebApplication app) {
            using var scoped = app.Services.CreateScope();
            
            var dbContext = scoped.ServiceProvider.GetService<AppDbContext>();
            var logger = scoped.ServiceProvider.GetRequiredService<ILogger<MigrationsHandler>>();

            try {
                dbContext.Database.Migrate();
                logger.LogInformation("Migrations were applied successfully");
            }   catch (System.Exception ex) {
                logger.LogError("Could not apply migrations: " + ex.Message);
            }         
        }
    }
}