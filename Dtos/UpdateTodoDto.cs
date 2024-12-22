using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TodoAPI.Dtos
{
    public class UpdateTodoDto
    {
        [Required]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Todo List name must be between 3 and 30 characters long")]
        public string Name { get; set; }
    }
}