using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using TodoAPI.Dtos;
using TodoApiTests.Utils;
using Xunit;

namespace TodoApiTests
{
    public class LoginApiTests : IClassFixture<TodoApplicationFixture>
    {
        private readonly TodoApplicationFixture _fixture;
        private readonly HttpClient _client;

        private readonly string _signUpEndpoint = "/user/signup";
        private readonly string _loginEndpoint = "/auth/login";

        public LoginApiTests(TodoApplicationFixture fixture) {
            _fixture = fixture;
            _client = fixture.Client;
        }

        [Fact]
        public async Task LoginEndpoint_ReturnsSuccess_WhenValidData() {
            var createUserDto = UserGenerator.GenerateUser();

            await SignUp(createUserDto);

            var loginDto = new LoginDto{
                Email= createUserDto.Email,
                Password= createUserDto.Password
            };

            var res = await _client.PostAsJsonAsync(_loginEndpoint, loginDto);
            res.EnsureSuccessStatusCode();

            var data = await res.Content.ReadAsStringAsync();
            var user = JsonSerializer.Deserialize<UserDto>(data, new JsonSerializerOptions{ PropertyNameCaseInsensitive = true});
        
            Assert.NotNull(user);
            Assert.Equal(createUserDto.Email, user.Email);
            Assert.Equal(createUserDto.Username, user.Username);
        }

        [Fact]
        public async Task LoginEndpoint_Returns401_WhenInvalidEmail() {
            var createUserDto = UserGenerator.GenerateUser();

            await SignUp(createUserDto);

            var loginDto = new LoginDto{
                Email= "test_email@test.com",
                Password= createUserDto.Password
            };

            var res = await _client.PostAsJsonAsync(_loginEndpoint, loginDto);
            Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
        }

        [Fact]
        public async Task LoginEndpoint_Returns401_WhenInvalidPassword() {
            var createUserDto = UserGenerator.GenerateUser();

            await SignUp(createUserDto);

            var loginDto = new LoginDto{
                Email= createUserDto.Email,
                Password= "pass$123"
            };

            var res = await _client.PostAsJsonAsync(_loginEndpoint, loginDto);
            Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
        }

        private async Task SignUp(CreateUserDto dto) {
            var res = await _client.PostAsJsonAsync(_signUpEndpoint, dto);
            res.EnsureSuccessStatusCode();
        }
    }
}