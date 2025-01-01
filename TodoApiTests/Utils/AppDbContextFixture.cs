using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TodoAPI.Data;
using TodoAPI.Models;

namespace TodoApiTests.Utils
{
    public class AppDbContextFixture
    {
        public AppDbContext Context { get; }

        public AppDbContextFixture()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase("TestDb")
                .Options;

            Context = new AppDbContext(options);
            Context.Database.EnsureCreated();
            Context.Database.EnsureDeleted();
        }

        public User SeedUser(string userId) {
            var user = new User {
                Id = userId,
            };

            Context.Users.Add(user);
            Context.SaveChangesAsync();
            return user;
        }

        public Todo SeedTodo(User user, string name) {
            var todo = new Todo {
                Id= Guid.NewGuid(),
                User= user,
                Name= name,
            };

            Context.Todos.Add(todo);
            Context.SaveChangesAsync();
            return todo;
        }

        public void Dispose()
        {
            Context.Dispose();
        }
    }
}