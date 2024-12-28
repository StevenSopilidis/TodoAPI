using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TodoAPI.Models
{
    public class TodoItem
    {
        public Guid Id { get; set; }
        [ForeignKey(nameof(User))]
        public string UserId { get; set; }
        public User User { get; set; }
        [ForeignKey(nameof(Todo))]
        public Guid TodoId { get; set; }
        public Todo Todo { get; set; }
        public string Description { get; set; }
        public bool Completed { get; set; } = false;
    }
}