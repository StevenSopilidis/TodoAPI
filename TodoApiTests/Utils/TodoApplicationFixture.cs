using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TodoAPI.Data;

namespace TodoApiTests.Utils
{
    public class TodoApplicationFixture : IDisposable
    {
        private readonly TodoApiApplication _instance;

        public HttpClient Client => _instance.CreateClient();

        public TodoApplicationFixture() {
            _instance = TodoApiApplication.Instance;
        }

        public async Task ClearDatabase() {
            using var scoped = _instance.Services.CreateScope();
            var context = scoped.ServiceProvider.GetRequiredService<AppDbContext>();

            // Track changes before deletion
            var hasChanges = false;

            // Check if there are records to remove
            if (context.Users.Any())
            {
                context.Users.RemoveRange(context.Users);
                hasChanges = true;
            }

            if (context.Todos.Any())
            {
                context.Todos.RemoveRange(context.Todos);
                hasChanges = true;
            }

            if (context.TodoItems.Any())
            {
                context.TodoItems.RemoveRange(context.TodoItems);
                hasChanges = true;
            }

            if (hasChanges)
            {
                try
                {
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    // Log the exception and handle it
                    Console.WriteLine($"Concurrency issue occurred: {ex.Message}");
                }
            }
        }

        public void Dispose()
        {
            using var scoped = _instance.Services.CreateScope();
            var context = scoped.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Dispose();
        }
    }
}