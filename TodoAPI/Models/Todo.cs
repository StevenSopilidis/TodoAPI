using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TodoAPI.Models
{
    public class Todo
    {
        public Guid Id { get; set; }
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
        public User User { get; set; }
        public string Name { get; set; }
        public ICollection<TodoItem> TodoItems { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}