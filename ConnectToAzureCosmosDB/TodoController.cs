using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Functions.Worker;
using ConnectToAzureCosmosDB;
using Newtonsoft.Json;
using ConnectToAzureCosmosDB.DTOs;

namespace FunctionAppWithCosmosDB.Controllers
{
    public class TodoController
    {
        private readonly ITodosService _cosmosDBService;

        public TodoController(ITodosService cosmosDBService)
        {
            _cosmosDBService = cosmosDBService;
        }

        [Function("GetAllTodos")]
        public async Task<IActionResult> GetTodosAsync(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "todos")] HttpRequest req,
            ILogger log)
        {
            try
            {
                var items = await _cosmosDBService.GetItemsAsync();
                return new OkObjectResult(items);
            }
            catch (System.Exception ex)
            {
                log.LogError($"Error: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
        [Function("AddTodo")]
        public async Task<IActionResult> AddTodosAsync(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "todos")] HttpRequest req,
            ILogger log)
        {
            // Validate input
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            CreateTodo todo = JsonConvert.DeserializeObject<CreateTodo>(requestBody);
            todo.id = Guid.NewGuid().ToString();
            if (todo == null || todo.Tenantid == null)
            {
                log.LogError("Invalid request body.");
                return new BadRequestObjectResult("Invalid input: id and partitionKey are required.");
            }

            try
            {
                CreateTodo createdTodo = await _cosmosDBService.AddItemAsync(todo);
                return new OkObjectResult(createdTodo);
            }
            catch (System.Exception ex)
            {
                log.LogError($"Error: {ex.Message}");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }
        }
    }
}