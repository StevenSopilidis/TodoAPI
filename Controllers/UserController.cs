using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TodoAPI.Dtos;
using TodoAPI.Models;
using TodoAPI.Repositories;
using TodoAPI.Services;

namespace TodoAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserRepo _userRepo;

        public UserController(IUserRepo userRepo, IMapper mapper, ILogger<UserController> logger)
        {
            _logger = logger;
            _userRepo = userRepo;
        }

        [AllowAnonymous]
        [HttpPost("signup")]
        public async Task<IActionResult> SignUp(CreateUserDto dto) {
            var exists = await _userRepo.UsernameOrEmailUsedAsync(dto.Username, dto.Email);
            if (exists)
                return BadRequest("Email or username specified already used");

            var errors = await _userRepo.CreateUserAsync(dto);
            if (errors.Any())
                return BadRequest(errors);

            return CreatedAtAction(nameof(AuthController.Login), new UserCreatedDto{
                Username= dto.Username, 
                Email= dto.Email
            });
        }


    }
}