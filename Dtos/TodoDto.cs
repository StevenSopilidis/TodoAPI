using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoAPI.Models;

namespace TodoAPI.Dtos
{
    public class TodoDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public UserDto User { get; set; }
        public ICollection<TodoItemDto> TodoItems { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}