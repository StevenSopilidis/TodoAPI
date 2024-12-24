using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using TodoAPI.Dtos;
using TodoAPI.Models;
using TodoAPI.Repositories;

namespace TodoAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class TodosController : Controller
    {
        private readonly ITodoRepo _todoRepo;
        private readonly ITodoItemRepo _todoItemRepo;
        private readonly IMapper _mapper;

        public TodosController(IMapper mapper, ITodoRepo todoRepo, ITodoItemRepo todoItemrepo) {
            _todoRepo = todoRepo;
            _todoItemRepo = todoItemrepo;
            _mapper = mapper;
        }

        [HttpPost("")]
        public async Task<IActionResult> PostTodo(CreateTodoDto dto) {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var todo = await _todoRepo.CreateTodoAsync(dto, userId);

            if (todo is null)
                return BadRequest("Could not create todo");

            return Ok(todo);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTodo(Guid id) {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var todo = await _todoRepo.GetTodoWithItemsAsync(id, userId);

            if (todo is null)
                return NotFound();

            return Ok(todo);
        }


        [HttpGet]
        public async Task<IActionResult> GetTodos() {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var todos = await _todoRepo.GetTodosAsync(userId);
            return Ok(todos);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTodo([FromRoute]Guid id, [FromBody]UpdateTodoDto dto) {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            var todo = await _todoRepo.GetTodoAsync(id, userId);
            if (todo is null) 
                return NotFound();
            
            var updated = await _todoRepo.UpdateTodoAsync(_mapper.Map<Todo>(todo), dto);
            if (!updated)
                return BadRequest("Could not update todo");

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodo(Guid id) {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var todo = await _todoRepo.GetTodoAsync(id, userId);
            if (todo is null)
                return NotFound();

            var deleted = await _todoRepo.DeleteTodoAsync(_mapper.Map<Todo>(todo));
            if (!deleted)
                return BadRequest("Could not delete todo");

            return NoContent();
        }

        [HttpPost("{id}/items")]
        public async Task<IActionResult> PostTodoItem(Guid id, CreateTodoItemDto dto) {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var todo = await _todoRepo.GetTodoWithItemsAsync(id, userId);

            if (todo is null)
                return NotFound();
            
            var todoItem = await _todoItemRepo.CreateTodoItemAsync(userId, _mapper.Map<Todo>(todo), dto);
            if (todoItem is null)
                return BadRequest("Could not create todo");

            return Ok();
        }
    }
}