using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using TodoAPI.Data;
using TodoAPI.Dtos;
using TodoAPI.Models;

namespace TodoAPI.Repositories
{
    public class TodoItemRepo : ITodoItemRepo
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<TodoItemRepo> _logger;

        public TodoItemRepo(ILogger<TodoItemRepo> logger,AppDbContext context, IMapper mapper) {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }   

        public async Task<TodoItemDto?> CreateTodoItemAsync(string userId, Todo todo, CreateTodoItemDto dto)
        {
            var todoItem = _mapper.Map<TodoItem>(dto);
            
            todoItem.UserId = userId;
            todoItem.TodoId = todo.Id;

            _context.TodoItems.Add(todoItem);
            todo.TodoItems.Add(todoItem);

            var saved = await _context.SaveChangesAsync() > 0;
            if (!saved) {
                _logger.LogError("Could not create todo item");
                return null;
            }

            return _mapper.Map<TodoItemDto>(todoItem);
        }

        public Task<bool> DeleteTodoItemAsync(TodoItem todoItem)
        {
            throw new NotImplementedException();
        }

        public Task<TodoItemDto?> GetTodoItemAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateTodoItemAsync(TodoItem todoItem, UpdateTodoItemDto dto)
        {
            throw new NotImplementedException();
        }
    }
}