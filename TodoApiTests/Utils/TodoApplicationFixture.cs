using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

            // remove previous data
            context.Users.RemoveRange(context.Users);
            context.Todos.RemoveRange(context.Todos);
            context.TodoItems.RemoveRange(context.TodoItems);
            await context.SaveChangesAsync();
        }

        public void Dispose()
        {
            using var scoped = _instance.Services.CreateScope();
            var context = scoped.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Database.EnsureDeleted();
            context.Dispose();
        }
    }
}