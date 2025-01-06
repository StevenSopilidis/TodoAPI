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
    public class AuthController : Controller
    {
        private readonly IUserRepo _userRepo;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger<AuthController> _logger;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;

        public AuthController(IMapper mapper,IUserRepo userRepo, SignInManager<User> signInManager, ITokenService tokenService, ILogger<AuthController> logger)
        {
            _userRepo = userRepo;
            _signInManager = signInManager;
            _logger = logger;
            _mapper = mapper;
            _tokenService = tokenService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto) {
            var user = await _userRepo.GetUserAsync(dto.Email);
            if (user is null)
                return Unauthorized();

            var signIn = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
            if (signIn.Succeeded is false)
                return Unauthorized();

            var result = _mapper.Map<UserDto>(user);
            result.Token = _tokenService.CreateToken(user);

            return Ok(result); 
        }
        
    }
}