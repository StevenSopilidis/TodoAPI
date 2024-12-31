using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MockQueryable;
using Moq;
using Moq.EntityFrameworkCore;
using TodoAPI.Data;
using TodoAPI.Dtos;
using TodoAPI.Models;
using TodoAPI.Repositories;
using Xunit;

namespace TodoApiTests
{
    public class TodoRepoTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<TodoRepo>> _loggerMock;
        private readonly Mock<AppDbContext> _contextMock;
        private readonly AppDbContext _context;
        private readonly TodoRepo _todoRepo;
        private readonly string _validUserId = "user-id";

        public TodoRepoTests() {
            _mapperMock = new Mock<IMapper>();

            _loggerMock = new Mock<ILogger<TodoRepo>>();

            // create in memory db
            var opts = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase("TestDb")
                .Options;

            _context = new AppDbContext(opts);
            _context.Database.EnsureDeleted();
            _context.Database.EnsureDeleted();
            _context.Users.Add(new User { Id = _validUserId });
            _context.SaveChanges();

            _todoRepo = new TodoRepo(_mapperMock.Object, _loggerMock.Object, _context);
        }   

        [Fact]
        public async Task CreateTodoAsync_ReturnsTodo_WhenTodoCreationSucceeds() {
            var todoName = "name";
            await CreateTodo(todoName);
        }

        [Fact]
        public async Task GetTodoAsync_ReturnsTodo_WhenTodoExists() {
            var todoName = "name";
            var createdTodo = await CreateTodo(todoName);

            var todo = await _todoRepo.GetTodoAsync(createdTodo.Id, _validUserId);

            Assert.NotNull(todo);
            Assert.Equal(todoName, todo.Name);
            Assert.Equal(_validUserId, todo.UserId);
        }

        [Fact]
        public async Task GetTodoAsync_ReturnsNull_WhenTodoDoesNotExists() {
            var todo = await _todoRepo.GetTodoAsync(Guid.NewGuid(), _validUserId);

            Assert.Null(todo);
        }

        [Fact]
        public async Task GetTodoAsync_ReturnsNull_WhenTodoDoesNotBelongToUser() {
            var todoName = "name";
            var invalidUserId = "invalid-id";
            var createdTodo = await CreateTodo(todoName);

            var todo = await _todoRepo.GetTodoAsync(createdTodo.Id, invalidUserId);

            Assert.Null(todo);
        }


        [Fact]
        public async Task UpdateTodoAsync_ReturnsTrue_WhenTodoWasUpdated() {
            var todoName = "todoName";
            var dto = await CreateTodo(todoName);

            var todo = _context.Todos.SingleOrDefault(todo => todo.Name == todoName);

            Assert.NotNull(todo);

            var updatedTodoDto = new UpdateTodoDto{
                Name = "UpdatedName"
            };
            var updated = await _todoRepo.UpdateTodoAsync(todo, updatedTodoDto);
            
            Assert.True(updated);

            var updatedDto = await _todoRepo.GetTodoAsync(todo.Id, _validUserId);
            Assert.NotNull(updatedDto);
            Assert.Equal(updatedTodoDto.Name, updatedDto.Name);
        }

        [Fact]
        public async Task DeleteTodoAsync_ReturnsTrue_WhenTodoWasDeleted() {
            var todoName = "todoName";
            var dto = await CreateTodo(todoName);

            var todo = _context.Todos.SingleOrDefault(todo => todo.Name == todoName);

            Assert.NotNull(todo);

            var deleted = await _todoRepo.DeleteTodoAsync(todo);
            Assert.True(deleted);

            var deletedTodo = await _todoRepo.GetTodoAsync(todo.Id, _validUserId);
            Assert.Null(deletedTodo);
        }

        private async Task<TodoDto> CreateTodo(string name) {
            var todoId = Guid.NewGuid();

            var dto = new CreateTodoDto {
                Name= name,
            };

            var todoObj = new Todo {
                Id = todoId,
                Name= name,
            };

            _mapperMock.Setup(m => m.Map<Todo>(dto)).Returns(todoObj);
            _mapperMock.Setup(m => m.Map<TodoDto>(todoObj)).Returns(new TodoDto {Name = name, Id=todoId});

            var todo = await _todoRepo.CreateTodoAsync(dto, _validUserId);

            Assert.NotNull(todo);
            Assert.Equal(todoId, todo.Id);
            Assert.Equal(name, todo.Name);

            return todo;
        }
    }
}