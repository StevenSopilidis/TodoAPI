using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TodoAPI.Data;
using TodoAPI.Dtos;
using TodoAPI.Models;

namespace TodoAPI.Repositories
{
    public class TodoRepo : ITodoRepo
    {
        private readonly IMapper _mapper;
        private readonly ILogger<TodoRepo> _logger;
        private readonly AppDbContext _context;

        public TodoRepo(IMapper mapper, ILogger<TodoRepo> logger, AppDbContext context) {
            _mapper = mapper;
            _logger = logger;
            _context = context;
        }

        public async Task<TodoDto?> CreateTodoAsync(CreateTodoDto dto, Guid userId)
        {
            var todo = _mapper.Map<Todo>(dto);
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Id == userId.ToString());
            todo.User = user;

            _context.Todos.Add(todo);
            var created = await _context.SaveChangesAsync() > 0;

            if (!created) {
                _logger.LogError("Could not create Todo");
                return null;
            }

            return _mapper.Map<TodoDto>(todo);
        }

        public async Task<bool> DeleteTodoAsync(Todo todo)
        {
            _context.Todos.Remove(todo);
            var removed = await _context.SaveChangesAsync() > 0;

            if (!removed)
                _logger.LogError("Could not remove todo: " + todo.Id);

            return removed;
        }

        public async Task<TodoDto?> GetTodoAsync(Guid id, Guid userId)
        {
            var todo = await _context.Todos.AsNoTracking().Include(t => t.User).SingleOrDefaultAsync(t => t.Id == id);

            if (todo is null)
                return null;

            if (todo.User.Id != userId.ToString())
                return null;

            return _mapper.Map<TodoDto>(todo);
        }

        public async Task<ICollection<TodoDto>> GetTodosAsync(Guid userId)
        {
            return await _context.Todos.Select(todo => todo.User.Id == userId.ToString()).ProjectTo<TodoDto>(_mapper.ConfigurationProvider).ToListAsync();
        }

        public async Task<bool> UpdateTodoAsync(Todo todo, UpdateTodoDto dto)
        {
            todo.Name = dto.Name;
            _context.Todos.Update(todo);
            var updated = await _context.SaveChangesAsync() > 0;
            
            if (!updated)
                _logger.LogError("Could not update todo: " + todo.Id);
            
            return updated;
        }
    }
}