using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoAPI.Dtos
{
    public class UpdateTodoItemDto
    {
        public bool Completed { get; set; }
        public string Description { get; set; }
        
    }
}