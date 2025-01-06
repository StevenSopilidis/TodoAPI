using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;
using TodoAPI.Dtos;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Net;
using Microsoft.AspNetCore.Http;
using TodoApiTests.Utils;

namespace TodoApiTests
{
    public class UserApiTests : IClassFixture<TodoApplicationFixture>
    {
        private readonly TodoApplicationFixture _fixture;
        private readonly HttpClient _client;

        private readonly string _endpoint = "/user/signup";

        public UserApiTests(TodoApplicationFixture fixture) {
            _fixture = fixture;
            _client = fixture.Client;
        }
        
        [Fact]
        public async Task SignUpEndpoint_ReturnsSuccess_WhenValidData() {
            var request = UserGenerator.GenerateUser();

            var res = await _client.PostAsJsonAsync(_endpoint, request);
            res.EnsureSuccessStatusCode();

            var jsonString = await res.Content.ReadAsStringAsync();
            var userCreated = JsonSerializer.Deserialize<UserCreatedDto>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.NotNull(userCreated);
            Assert.Equal(request.Email, userCreated.Email);
            Assert.Equal(request.Username, userCreated.Username);
        }

        [Fact]
        public async Task SignUpEndpoint_Returns400_WhenInvalidUsernane() {
            var request = new CreateUserDto{
                Username= "t", // username must be between 3 and 20 chars
                Email= "test@test.com",
                Password= "pass1234@",
            };

            var res = await _client.PostAsJsonAsync(_endpoint, request);
            Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
        }

        [Fact]
        public async Task SignUpEndpoint_Returns400_WhenInvalidEmail() {
            var request = new CreateUserDto{
                Username= "test",
                Email= "invalid-email",
                Password= "pass1234@",
            };

            var res = await _client.PostAsJsonAsync(_endpoint, request);
            Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
        }
        
        [Fact]
        public async Task SignUpEndpoint_Returns400_WhenInvalidPassword() {
            var request = new CreateUserDto{
                Username= "test",
                Email= "test@test.com",
                Password= "sd", // invalid
            };

            var res = await _client.PostAsJsonAsync(_endpoint, request);
            Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
        }

        [Fact]
        public async Task SignUpEndpoint_Returns400_WhenUsernameOrEmailAlreadyTaken() {
            var request = UserGenerator.GenerateUser();

            var res = await _client.PostAsJsonAsync(_endpoint, request);
            res.EnsureSuccessStatusCode();

            var jsonString = await res.Content.ReadAsStringAsync();
            var userCreated = JsonSerializer.Deserialize<UserCreatedDto>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.NotNull(userCreated);
            Assert.Equal(request.Email, userCreated.Email);
            Assert.Equal(request.Username, userCreated.Username);

            res = await _client.PostAsJsonAsync(_endpoint, request);
            Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
        }
    }
}