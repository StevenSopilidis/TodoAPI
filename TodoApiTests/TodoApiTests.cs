using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using TodoAPI.Dtos;
using TodoApiTests.Utils;

namespace TodoApiTests
{
    public class TodoApiTests : IClassFixture<TodoApplicationFixture>
    {
        private readonly TodoApplicationFixture _fixture;
        private readonly HttpClient _client;
        private readonly string _signUpEndpoint = "/user/signup";
        private readonly string _loginEndpoint = "/auth/login";
        private readonly string _createTodoEndpoint = "/todos";
        private readonly Func<string, string> _getTodoEndpoint = (todoId) => $"/todos/{todoId}";
        private readonly Func<string, string> _updateTodoEndpoint = (todoId) => $"/todos/{todoId}";
        private readonly Func<string, string> _deleteTodoEndpoint = (todoId) => $"/todos/{todoId}";
        private readonly Func<string, string> _createTodoItemEndpoint = (todoId) => $"/todos/{todoId}/items";
        private readonly Func<string, string, string> _getTodoItemEndpoint =
            (todoId, todoItemId) => $"/todos/{todoId}/items/{todoItemId}";
        private readonly Func<string, string, string> _updateTodoItemEndpoint =
            (todoId, todoItemId) => $"/todos/{todoId}/items/{todoItemId}";
        private readonly Func<string, string, string> _deleteTodoItemEndpoint =
            (todoId, todoItemId) => $"/todos/{todoId}/items/{todoItemId}";

        private readonly string _getTodos = "/todos";

        public TodoApiTests(TodoApplicationFixture fixture) {
            _fixture = fixture;
            _client = fixture.Client;
        }

        [Fact]
        public async Task PostTodo_Returns200_WhenValidDataProvided() {
            await SignUpAndLogin();

            var dto = new CreateTodoDto{
                Name= "name"
            };
            await CreateValidTodo(dto);
        }

        [Fact]
        public async Task PostTodo_Returns401_WhenUserProvidedNoAuthorizationHeader() {
            var dto = new CreateTodoDto{
                Name= "name"
            };
            var res = await _client.PostAsJsonAsync(_createTodoEndpoint, dto);
            Assert.Equal(HttpStatusCode.Unauthorized, res.StatusCode);
        }

        [Fact]
        public async Task PostTodo_Returns400_WhenInvalidName() {
            await SignUpAndLogin();

            var dto = new CreateTodoDto{
                Name= "t"
            };
            var res = await _client.PostAsJsonAsync(_createTodoEndpoint, dto);
            Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
        }

        [Fact]
        public async Task GetTodo_Returns200_WhenTodoExists() {
            await SignUpAndLogin();

            var dto = new CreateTodoDto{
                Name= "name"
            };
            var todoDto = await CreateValidTodo(dto);

            var res = await _client.GetAsync(_getTodoEndpoint(todoDto.Id.ToString()));
            res.EnsureSuccessStatusCode();
            var dtoRes = await ParseResponse<TodoDto>(res);
            Assert.NotNull(dtoRes);
            Assert.Equal(dto.Name, dtoRes.Name);
            Assert.Empty(dtoRes.TodoItems);
        }

        [Fact]
        public async Task GetTodo_Returns404_WhenTodoDoesNotExist() {
            await SignUpAndLogin();

            var dto = new CreateTodoDto{
                Name= "name"
            };
            var todoDto = await CreateValidTodo(dto);
            var res = await _client.GetAsync(_getTodoEndpoint(Guid.NewGuid().ToString()));
            Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
        }

        [Fact]
        public async Task GetTodo_Returns404_WhenUnauthrorizedUserTriesToAccessIt() {
            await SignUpAndLogin();

            var dto = new CreateTodoDto{
                Name= "name"
            };
            var todoDto = await CreateValidTodo(dto);

            await SignUpAndLogin();
            var res = await _client.GetAsync(_getTodoEndpoint(Guid.NewGuid().ToString()));
            Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
        }

        [Fact]
        public async Task GetTodo_Returns400_WhenInvalidIdProvided() {
            await SignUpAndLogin();
            
            // id must be Guid
            var res = await _client.GetAsync(_getTodoEndpoint("invalid-id"));
            Assert.Equal(HttpStatusCode.BadRequest, res.StatusCode);
        }

        [Fact]
        public async Task GetTodos_Returns_AllTodosOfUser() {
            await SignUpAndLogin();
            
            var todos = new List<CreateTodoDto>{
                new CreateTodoDto{
                    Name= "name1"
                },
                new CreateTodoDto {
                    Name= "name2"
                },
                new CreateTodoDto {
                    Name= "name3"
                },
            };

            foreach (var todo in todos)
            {
                await CreateValidTodo(todo);
            }

            var res = await _client.GetAsync(_getTodos);
            res.EnsureSuccessStatusCode();
            var stringData = await res.Content.ReadAsStringAsync();
            var body = JsonSerializer.Deserialize<ICollection<TodoDto>>(stringData, new JsonSerializerOptions{ PropertyNameCaseInsensitive= true });
            Assert.NotNull(body);
            Assert.Equal(todos.Count(), body.Count());
        }

        [Fact]
        public async Task UpdateTodo_Returns201_WhenSuccessfullyUpdatedTodo() {
            await SignUpAndLogin();
        
            var dto = new CreateTodoDto{
                Name= "name"
            };
            var todoDto = await CreateValidTodo(dto);

            var updateTodoDto = new UpdateTodoDto{
                Name= "updatedName"
            };

            var res = await _client.PutAsJsonAsync(_updateTodoEndpoint(todoDto.Id.ToString()), updateTodoDto);
            Assert.Equal(HttpStatusCode.NoContent, res.StatusCode);

            res = await _client.GetAsync(_getTodoEndpoint(todoDto.Id.ToString()));
            res.EnsureSuccessStatusCode();
            var dtoRes = await ParseResponse<TodoDto>(res);
            Assert.NotNull(dtoRes);
            Assert.Equal(updateTodoDto.Name, dtoRes.Name);
        }

        [Fact]
        public async Task UpdateTodo_Returns404_WhenInvalidId() {
            await SignUpAndLogin();
            
            var updateDto = new UpdateTodoDto {
                Name = "Updated Name"
            };

            var res = await _client.PutAsJsonAsync(_updateTodoEndpoint(Guid.NewGuid().ToString()), updateDto);
            Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
        }

        [Fact]
        public async Task UpdateTodo_Returns404_WhenInvalidAnauthroized() {
           await SignUpAndLogin();

            var dto = new CreateTodoDto{
                Name= "name"
            };
            var todoDto = await CreateValidTodo(dto);

            var updateDto = new UpdateTodoDto {
                Name = "Updated Name"
            };
            
            await SignUpAndLogin();
            var res = await _client.PutAsJsonAsync(_updateTodoEndpoint(todoDto.Id.ToString()), updateDto);
            Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
        }

        [Fact]
        public async Task DeleteTodo_Returns201_WhenSuccessfullyDeletedTodo() {
            await SignUpAndLogin();
        
            var dto = new CreateTodoDto{
                Name= "name"
            };
            var todoDto = await CreateValidTodo(dto);


            var res = await _client.DeleteAsync(_deleteTodoEndpoint(todoDto.Id.ToString()));
            Assert.Equal(HttpStatusCode.NoContent, res.StatusCode);

            res = await _client.GetAsync(_getTodoEndpoint(todoDto.Id.ToString()));
            Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
        }

        [Fact]
        public async Task DeleteTodo_Returns404_WhenInvalidAnauthroized() {
            await SignUpAndLogin();

            var dto = new CreateTodoDto{
                Name= "name"
            };
            var todoDto = await CreateValidTodo(dto);
            
            await SignUpAndLogin();
            var res = await _client.DeleteAsync(_deleteTodoEndpoint(todoDto.Id.ToString()));
            Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
        }

        [Fact]
        public async Task DeleteTodo_Returns404_WhenInvalidId() {
            await SignUpAndLogin();
            
            var res = await _client.DeleteAsync(_deleteTodoEndpoint(Guid.NewGuid().ToString()));
            Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
        }

        [Fact]
        public async Task PostTodoItem_Returns404_WhenInvalidTodoId() {
            await SignUpAndLogin();

            var dto = new CreateTodoItemDto{
                Description= "Test Description"
            };

            var res = await _client.PostAsJsonAsync(_createTodoItemEndpoint(Guid.NewGuid().ToString()), dto);
            Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
        }

        [Fact]
        public async Task PostTodoItem_Returns404_WhenUserTriesToCreateTodoItemToNotBelongingTodo() {
            await SignUpAndLogin();

            var dto = new CreateTodoDto{
                Name= "name"
            };
            var todoDto = await CreateValidTodo(dto);

            await SignUpAndLogin();

            var createTodoItemDto = new CreateTodoItemDto{
                Description= "Test description"
            };
            var res = await _client.PostAsJsonAsync(_createTodoItemEndpoint(todoDto.Id.ToString()), createTodoItemDto);
            Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
        }

        [Fact]
        public async Task PostTodoItem_Returns200_WhenTodoItemWasCreatedSuccessfully() {
            await SignUpAndLogin();

            var createTodoDto = new CreateTodoDto{
                Name= "name"
            };
            var todoDto = await CreateValidTodo(createTodoDto);

            var createTodoItemDto = new CreateTodoItemDto{
                Description= "Test item"
            };
            await CreateValidTodoItem(todoDto.Id, createTodoItemDto);
        }

        [Fact]
        public async Task GetTodoItem_Returns404_WhenTodoItemDoesNotExist() {
            await SignUpAndLogin();

            var createTodoDto = new CreateTodoDto{
                Name= "name"
            };
            var todoDto = await CreateValidTodo(createTodoDto);

            var res = await _client.GetAsync(_getTodoItemEndpoint(todoDto.Id.ToString(), Guid.NewGuid().ToString()));
            Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
        }

        [Fact]
        public async Task GetTodoItem_Returns404_WhenTodoDoesNotExist() {
            await SignUpAndLogin();

            var res = await _client.GetAsync(_getTodoItemEndpoint(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()));
            Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
        }

        [Fact]
        public async Task GetTodoItem_Returns404_WhenUserDoesNotOwnTodoItem() {
            await SignUpAndLogin();

            var createTodoDto = new CreateTodoDto{
                Name= "name"
            };
            var todoDto = await CreateValidTodo(createTodoDto);

            var createTodoItemDto = new CreateTodoItemDto{
                Description="Test"
            };
            var todoItemDto = await CreateValidTodoItem(todoDto.Id, createTodoItemDto);

            await SignUpAndLogin();
            var res = await _client.GetAsync(_getTodoItemEndpoint(todoDto.Id.ToString(), todoItemDto.Id.ToString()));
            Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
        }

        [Fact]
        public async Task GetTodoItem_Returns200_WhenEverythingRunsCorrectly() {
            await SignUpAndLogin();

            var createTodoDto = new CreateTodoDto{
                Name= "name"
            };
            var todoDto = await CreateValidTodo(createTodoDto);

            var createTodoItemDto = new CreateTodoItemDto{
                Description="Test"
            };
            var todoItemDto = await CreateValidTodoItem(todoDto.Id, createTodoItemDto);

            var res = await _client.GetAsync(_getTodoItemEndpoint(todoDto.Id.ToString(), todoItemDto.Id.ToString()));
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);

            var body = await ParseResponse<TodoItemDto>(res);
            Assert.NotNull(body);
            Assert.Equal(createTodoItemDto.Description, body.Description);
            Assert.False(body.Completed);
        }

        [Fact]
        public async Task PutTodoItem_Returns404_WhenTodoItemDoesNotExist() {
            await SignUpAndLogin();

            var createTodoDto = new CreateTodoDto{
                Name= "name"
            };
            var todoDto = await CreateValidTodo(createTodoDto);

            var updateTodoItemDto = new UpdateTodoItemDto{
                Description="Updated",
                Completed= true
            };

            var res = await _client.PutAsJsonAsync(
                _updateTodoItemEndpoint(todoDto.Id.ToString(), Guid.NewGuid().ToString()),
                 updateTodoItemDto
            );
            Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
        }

        [Fact]
        public async Task PutTodoItem_Returns404_WhenTodoDoesNotExist() {
            await SignUpAndLogin();

            var updateTodoItemDto = new UpdateTodoItemDto{
                Description="Updated",
                Completed= true
            };

            var res = await _client.PutAsJsonAsync(
                _updateTodoItemEndpoint(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()), 
                updateTodoItemDto
            );
            Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
        }

        [Fact]
        public async Task PutTodoItem_Returns404_WhenUserDoesNotOwnTodoItem() {
            await SignUpAndLogin();

            var createTodoDto = new CreateTodoDto{
                Name= "name"
            };
            var todoDto = await CreateValidTodo(createTodoDto);

            var createTodoItemDto = new CreateTodoItemDto{
                Description="Test"
            };
            var todoItemDto = await CreateValidTodoItem(todoDto.Id, createTodoItemDto);

            await SignUpAndLogin();

            var updateTodoItemDto = new UpdateTodoItemDto{
                Description="Updated",
                Completed= true
            };
            var res = await _client.PutAsJsonAsync(
                _updateTodoItemEndpoint(Guid.NewGuid().ToString(), Guid.NewGuid().ToString()), 
                updateTodoItemDto
            );
            Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
        }

        [Fact]
        public async Task PutTodoItem_Returns201_WhenEverythingRunsCorrectly() {
            await SignUpAndLogin();

            var createTodoDto = new CreateTodoDto{
                Name= "name"
            };
            var todoDto = await CreateValidTodo(createTodoDto);

            var createTodoItemDto = new CreateTodoItemDto{
                Description="Test"
            };
            var todoItemDto = await CreateValidTodoItem(todoDto.Id, createTodoItemDto);

            var updateTodoItemDto = new UpdateTodoItemDto{
                Description="Updated",
                Completed= true
            };
            var res = await _client.PutAsJsonAsync(
                _updateTodoItemEndpoint(todoDto.Id.ToString(), todoItemDto.Id.ToString()), 
                updateTodoItemDto
            );
            Assert.Equal(HttpStatusCode.NoContent, res.StatusCode);

            res = await _client.GetAsync(_getTodoItemEndpoint(todoDto.Id.ToString(), todoItemDto.Id.ToString()));
            Assert.Equal(HttpStatusCode.OK, res.StatusCode);

            var body = await ParseResponse<TodoItemDto>(res);
            Assert.NotNull(body);
            Assert.Equal(updateTodoItemDto.Description, body.Description);
            Assert.Equal(updateTodoItemDto.Completed, body.Completed);
        }

        [Fact]
        public async Task DeleteTodoItem_Returns404_WhenTodoItemDoesNotExist() {
            await SignUpAndLogin();

            var createTodoDto = new CreateTodoDto{
                Name= "name"
            };
            var todoDto = await CreateValidTodo(createTodoDto);

            var res = await _client.DeleteAsync(
                _updateTodoItemEndpoint(todoDto.Id.ToString(), Guid.NewGuid().ToString())
            );
            Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
        }

        [Fact]
        public async Task DeleteTodoItem_Returns404_WhenTodoDoesNotExist() {
            await SignUpAndLogin();

            var updateTodoItemDto = new UpdateTodoItemDto{
                Description="Updated",
                Completed= true
            };

            var res = await _client.DeleteAsync(
                _updateTodoItemEndpoint(Guid.NewGuid().ToString(), Guid.NewGuid().ToString())
            );
            Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
        }

        [Fact]
        public async Task DeleteTodoItem_Returns404_WhenUserDoesNotOwnTodoItem() {
            await SignUpAndLogin();

            var createTodoDto = new CreateTodoDto{
                Name= "name"
            };
            var todoDto = await CreateValidTodo(createTodoDto);

            var createTodoItemDto = new CreateTodoItemDto{
                Description="Test"
            };
            var todoItemDto = await CreateValidTodoItem(todoDto.Id, createTodoItemDto);

            await SignUpAndLogin();

            var res = await _client.DeleteAsync(
                _updateTodoItemEndpoint(todoDto.Id.ToString(), todoItemDto.Id.ToString())
            );
            Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
        }

        [Fact]
        public async Task DeleteTodoItem_Returns201_WhenEverythingRunsCorrectly() {
            await SignUpAndLogin();

            var createTodoDto = new CreateTodoDto{
                Name= "name"
            };
            var todoDto = await CreateValidTodo(createTodoDto);

            var createTodoItemDto = new CreateTodoItemDto{
                Description="Test"
            };
            var todoItemDto = await CreateValidTodoItem(todoDto.Id, createTodoItemDto);

            var res = await _client.DeleteAsync(
                _updateTodoItemEndpoint(todoDto.Id.ToString(), todoItemDto.Id.ToString())
            );
            Assert.Equal(HttpStatusCode.NoContent, res.StatusCode);

            res = await _client.GetAsync(_getTodoItemEndpoint(todoDto.Id.ToString(), todoItemDto.Id.ToString()));
            Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);
        }


        private async Task SignUpAndLogin() {
            var createUserDto = UserGenerator.GenerateUser();
            
            await CreateUser(createUserDto);
            var bearerToken = await Login(createUserDto.Email, createUserDto.Password);
            
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);
        }

        private async Task CreateUser(CreateUserDto dto) {
            var res = await _client.PostAsJsonAsync(_signUpEndpoint, dto);
            res.EnsureSuccessStatusCode();

            var jsonString = await res.Content.ReadAsStringAsync();
            var userCreated = JsonSerializer.Deserialize<UserCreatedDto>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.NotNull(userCreated);
            Assert.Equal(dto.Email, userCreated.Email);
            Assert.Equal(dto.Username, userCreated.Username);
        }

        private async Task<string> Login(string email, string password) {
            var loginDto = new LoginDto{
                Email= email,
                Password= password
            };

            var res = await _client.PostAsJsonAsync(_loginEndpoint, loginDto);
            res.EnsureSuccessStatusCode();

            var data = await res.Content.ReadAsStringAsync();
            var user = JsonSerializer.Deserialize<UserDto>(data, new JsonSerializerOptions{ PropertyNameCaseInsensitive = true});

            Assert.NotNull(user);
            Assert.Equal(email, user.Email);

            return user.Token;
        }

        private async Task<TodoDto> CreateValidTodo(CreateTodoDto dto) {
            var res = await _client.PostAsJsonAsync(_createTodoEndpoint, dto);
            res.EnsureSuccessStatusCode();

            var dtoRes = await ParseResponse<TodoDto>(res);
            Assert.NotNull(dtoRes);
            Assert.Equal(dto.Name, dtoRes.Name);
            Assert.Empty(dtoRes.TodoItems);

            return dtoRes;
        }

        private async Task<TodoItemDto> CreateValidTodoItem(Guid todoId, CreateTodoItemDto dto) {
            var res = await _client.PostAsJsonAsync(_createTodoItemEndpoint(todoId.ToString()), dto);
            res.EnsureSuccessStatusCode();

            var dtoRes = await ParseResponse<TodoItemDto>(res);
            Assert.NotNull(dtoRes);
            Assert.Equal(dto.Description, dtoRes.Description);
            Assert.False(dtoRes.Completed);

            return dtoRes;
        }

        private async Task<T?> ParseResponse<T>(HttpResponseMessage res) {
            var jsonString = await res.Content.ReadAsStringAsync();
            var dtoRes = JsonSerializer.Deserialize<T>(jsonString, new JsonSerializerOptions{ PropertyNameCaseInsensitive = true});
            return dtoRes;
        }
    }
}