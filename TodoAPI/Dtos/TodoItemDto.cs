using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoAPI.Dtos
{
    public class TodoItemDto
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public bool Completed { get; set; }
    }
}