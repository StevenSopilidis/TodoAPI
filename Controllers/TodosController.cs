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
        private readonly IMapper _mapper;

        public TodosController(IMapper mapper, ITodoRepo todoRepo) {
            _todoRepo = todoRepo;
            _mapper = mapper;
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

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTodo([FromRoute]Guid id, [FromBody]UpdateTodoDto dto) {
            var userId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));
            
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
            var userId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var todo = await _todoRepo.GetTodoAsync(id, userId);
            if (todo is null)
                return NotFound();

            var deleted = await _todoRepo.DeleteTodoAsync(_mapper.Map<Todo>(todo));
            if (!deleted)
                return BadRequest("Could not delete todo");

            return NoContent();
        }
    }
}