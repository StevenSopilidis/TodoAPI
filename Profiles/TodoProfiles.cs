using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using TodoAPI.Dtos;
using TodoAPI.Models;

namespace TodoAPI.Profiles
{
    public class TodoProfiles : Profile
    {
        public TodoProfiles() {
            CreateMap<CreateTodoDto, Todo>();
            CreateMap<Todo, TodoDto>();
            CreateMap<TodoDto, Todo>();
            CreateMap<TodoItem, TodoItemDto>();
            CreateMap<TodoItemDto, TodoItem>();
            CreateMap<CreateTodoItemDto, TodoItem>();
        }
    }
}