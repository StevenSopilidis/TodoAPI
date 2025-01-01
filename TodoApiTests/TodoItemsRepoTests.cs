using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using Moq;
using TodoAPI.Data;
using TodoAPI.Dtos;
using TodoAPI.Models;
using TodoAPI.Repositories;
using TodoApiTests.Utils;
using Xunit;

namespace TodoApiTests
{
    public class TodoItemsRepoTests : IClassFixture<AppDbContextFixture>
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<TodoItemRepo>> _loggerMock;
        private readonly AppDbContext _context;
        private readonly string _validUserId = Guid.NewGuid().ToString();
        private readonly Guid _todoId;
        private readonly string _todoName = Guid.NewGuid().ToString();
        private readonly Todo _testTodo;
        private readonly TodoItemRepo _todoItemsRepo;

        public TodoItemsRepoTests(AppDbContextFixture fixture) {
            _mapperMock = new Mock<IMapper>();

            _loggerMock = new Mock<ILogger<TodoItemRepo>>();

            // create in memory db
            _context = fixture.Context;

            // seed user
            var user = fixture.SeedUser(_validUserId);
            // seed todo
            _testTodo = fixture.SeedTodo(user, _todoName);
            _todoId = _testTodo.Id;

            _context.SaveChanges();

            _todoItemsRepo = new TodoItemRepo(_loggerMock.Object, _context, _mapperMock.Object);
        }

        [Fact]
        private async Task CreateTodoItemAsync_ReturnsTodoItem_WhenSuccessful() {
            var description = "TodoItem";
            await CreateTodoItem(description);
        }

        [Fact]
        private async Task GetTodoItemAsync_ReturnsTodoItem_WhenExists() {
            var description = "TodoItem";
            var item = await CreateTodoItem(description);

            var todo = await _todoItemsRepo.GetTodoItemAsync(item.Id, _todoId, _validUserId);
            Assert.NotNull(todo);
            Assert.Equal(item.Description, todo.Description);
            Assert.Equal(_todoId, todo.TodoId);
            Assert.Equal(_validUserId, todo.UserId);
        }

        [Fact]
        private async Task GetTodoItemAsync_ReturnsNull_WhenUserDoesNotOwnPost() {
            var description = "TodoItem";
            var item = await CreateTodoItem(description);

            var todo = await _todoItemsRepo.GetTodoItemAsync(item.Id, _todoId, "invalid-user-id");
            Assert.Null(todo);
        }

        [Fact]
        private async Task GetTodoItemAsync_ReturnsNull_WhenTodoDoesNotOwnPost() {
            var description = "TodoItem";
            var item = await CreateTodoItem(description);

            var todo = await _todoItemsRepo.GetTodoItemAsync(item.Id, Guid.NewGuid(), _validUserId);
            Assert.Null(todo);
        }

        [Fact]
        private async Task DeleteTodoItemAsync_ReturnsTrue_WhenTodoIsDeleted() {
            var description = "desc";
            var todoItemDto = await CreateTodoItem(description);

            var todoItem = await _todoItemsRepo.GetTodoItemAsync(todoItemDto.Id, _todoId, _validUserId);
            Assert.NotNull(todoItem);

            var deleted = await _todoItemsRepo.DeleteTodoItemAsync(todoItem);
            Assert.True(deleted);

            var deletedItem = await _todoItemsRepo.GetTodoItemAsync(todoItemDto.Id, _todoId, _validUserId);
            Assert.Null(deletedItem);
        }

        [Fact]
        private async Task UpdateTodoItemAsync_ReturnsTrue_WhenTodoWasUpdated() {
            var description = "desc";
            var todoItemDto = await CreateTodoItem(description);

            var todoItem = await _todoItemsRepo.GetTodoItemAsync(todoItemDto.Id, _todoId, _validUserId);
            Assert.NotNull(todoItem);

            var updateItemDto = new UpdateTodoItemDto {
                Description= "New Description",
                Completed= true,
            };

            var updated = await _todoItemsRepo.UpdateTodoItemAsync(todoItem, updateItemDto);
            Assert.True(updated);

            var updatedItem = await _todoItemsRepo.GetTodoItemAsync(todoItemDto.Id, _todoId, _validUserId);
            Assert.NotNull(updatedItem);
            Assert.Equal(updateItemDto.Description, updatedItem.Description);
            Assert.Equal(updateItemDto.Completed, updatedItem.Completed);
        }

        private async Task<TodoItemDto> CreateTodoItem(string description) {
            var todoItemId = Guid.NewGuid();
            
            var dto = new CreateTodoItemDto {
                Description = description
            };

            var todoItem = new TodoItem {
                Id= todoItemId,
                UserId= _validUserId,
                TodoId= _todoId,
                Description= description,
            };

            _mapperMock.Setup(m => m.Map<TodoItem>(dto)).Returns(todoItem);
            _mapperMock.Setup(m => m.Map<TodoItemDto>(todoItem)).Returns(new TodoItemDto{
                Id= todoItemId,
                Description= description,
                Completed= false,
            });

            var item = await _todoItemsRepo.CreateTodoItemAsync(_validUserId, _testTodo, dto);
            Assert.NotNull(item);
            Assert.Equal(todoItemId, item.Id);
            Assert.Equal(description, item.Description);
            Assert.False(item.Completed);

            return item;
        }
    }
}