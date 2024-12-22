using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using TodoAPI.Dtos;
using TodoAPI.Repositories;

namespace TodoAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class TodosController : Controller
    {
        private readonly ITodoRepo _todoRepo;

        public TodosController(ITodoRepo todoRepo) {
            _todoRepo = todoRepo;
        }

        [HttpPost("")]
        public async Task<IActionResult> CreateTodo(CreateTodoDto dto) {
            var userId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var todo = await _todoRepo.CreateTodoAsync(dto, userId);

            if (todo is null)
                return BadRequest("Could not create todo");

            return Ok(todo);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTodo(Guid id) {
            var userId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var todo = await _todoRepo.GetTodoAsync(id, userId);

            if (todo is null)
                return NotFound();

            return Ok(todo);
        }


        [HttpGet]
        public async Task<IActionResult> GetTodos() {
            var userId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var todos = await _todoRepo.GetTodosAsync(userId);
            return Ok(todos);
        }
    }
}