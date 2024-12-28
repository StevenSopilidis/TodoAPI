using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoAPI.Dtos;
using TodoAPI.Models;

namespace TodoAPI.Repositories
{
    public interface ITodoRepo
    {
        Task<TodoDto?> CreateTodoAsync(CreateTodoDto dto, string userId);
        Task<Todo?> GetTodoAsync(Guid id, string userId);
        Task<TodoDto?> GetTodoWithItemsAsync(Guid id, string userId);
        Task<ICollection<TodoDto>> GetTodosAsync(string userId);
        Task<bool> UpdateTodoAsync(Todo todo, UpdateTodoDto dto);
        Task<bool> DeleteTodoAsync(Todo todo);
    }
}