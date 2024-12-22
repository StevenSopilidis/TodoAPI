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
        Task<TodoDto?> CreateTodoAsync(CreateTodoDto dto, Guid userId);
        Task<TodoDto?> GetTodoAsync(Guid id, Guid userId);
        Task<ICollection<TodoDto>> GetTodosAsync(Guid userId);
        Task<bool> UpdateTodoAsync(Todo todo, UpdateTodoDto dto);
        Task<bool> DeleteTodoAsync(Todo todo);
    }
}