using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoAPI.Dtos;
using TodoAPI.Models;

namespace TodoAPI.Repositories
{
    public interface ITodoItemRepo
    {
        Task<TodoItem?> GetTodoItemAsync(Guid id, Guid todoId, string userId);
        Task<TodoItemDto?> CreateTodoItemAsync(string userId, Todo todo, CreateTodoItemDto dto);
        Task<bool> DeleteTodoItemAsync(TodoItem todoItem);
        Task<bool> UpdateTodoItemAsync(TodoItem todoItem, UpdateTodoItemDto dto);
    }
}