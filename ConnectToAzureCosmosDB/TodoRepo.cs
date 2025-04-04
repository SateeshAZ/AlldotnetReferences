using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConnectToAzureCosmosDB.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ConnectToAzureCosmosDB
{
    public interface ITodoRepo
    {
        Task<List<dynamic>> GetItemsAsync();
        Task<CreateTodo> AddItemsAsync(CreateTodo todo, ILogger log);
    }

    public class TodoRepo : ITodoRepo
    {
        private readonly CosmosClient _cosmosClient;
        private readonly string _databaseId = "TodoDB";
        private readonly string _containerId = "TodoTable";

        public TodoRepo(CosmosClient cosmosClient)
        {
            _cosmosClient = cosmosClient;
        }

        public async Task<List<dynamic>> GetItemsAsync()
        {
            var container = _cosmosClient.GetContainer(_databaseId, _containerId);
            var query = "SELECT * FROM c";
            QueryDefinition queryDefinition = new QueryDefinition(query);
            var iterator = container.GetItemQueryIterator<ReadTodo>(queryDefinition);

            var results = new List<dynamic>();
            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                results.AddRange(response);
            }

            return results;
        }

        public async Task<CreateTodo> AddItemsAsync(CreateTodo todo, ILogger log)
        {
            // Read request body
            

            // Create item
            todo.id = Guid.NewGuid().ToString();
            try
            {
            var container = _cosmosClient.GetContainer(_databaseId, _containerId);
            ItemResponse<CreateTodo> itemResponse = await container.CreateItemAsync(todo, new PartitionKey(todo.Tenantid));
            }
            catch (Exception ex) {
                log.LogError($"Error: {ex.Message}");
            }
            return todo;
        }

    }
}
