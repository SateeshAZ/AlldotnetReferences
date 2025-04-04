using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConnectToAzureCosmosDB.DTOs;
using Microsoft.AspNetCore.Http;

namespace ConnectToAzureCosmosDB
{
    public interface ITodosService
    {
        Task<List<dynamic>> GetItemsAsync();
        Task<CreateTodo> AddItemAsync(CreateTodo todo);
    }

    public class TodoService : ITodosService
    {
        private readonly ITodoRepo _repository;

        public TodoService(ITodoRepo repository)
        {
            _repository = repository;
        }

        public async Task<List<dynamic>> GetItemsAsync()
        {
            return await _repository.GetItemsAsync();
        }
        public async Task<CreateTodo> AddItemAsync(CreateTodo todo)
        {
            return await _repository.AddItemsAsync(todo, null);
        }
    }
}
