using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TodoAPI.Models
{
    public class TodoItem
    {
        public Guid Id { get; set; }
        public User User { get; set; }
        public string Description { get; set; }
        public bool Completed { get; set; }
    }
}