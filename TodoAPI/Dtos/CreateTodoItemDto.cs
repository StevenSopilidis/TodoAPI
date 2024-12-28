using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TodoAPI.Dtos
{
    public class CreateTodoItemDto
    {
        [StringLength(300, MinimumLength = 3, ErrorMessage = "Todo item desciption must be between 3 and 300 characters long")]
        public string Description { get; set; }
    }
}