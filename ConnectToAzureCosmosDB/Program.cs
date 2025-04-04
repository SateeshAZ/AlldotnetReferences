using ConnectToAzureCosmosDB;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// Cosmos DB connection
string cosmosDbConnectionString = Environment.GetEnvironmentVariable("CosmosDBConnection") ?? throw new InvalidOperationException("CosmosDBConnection is not set.");
// Cosmos DB Configuration
string databaseId = "TodoDB";
string containerId = "TodoTable";
string partitionKeyPath = "/Tenantid";
// Initialize CosmosClient
CosmosClient cosmosClient = new CosmosClient(cosmosDbConnectionString);
try
{
    // Ensure the database and container exist
    await InitializeCosmosDBAsync(cosmosClient);

    Console.WriteLine("Database and Container created or verified successfully.");
}
catch (CosmosException ex)
{
    Console.WriteLine($"Cosmos DB error: {ex.StatusCode} - {ex.Message}");
}
catch (Exception ex)
{
    Console.WriteLine($"General error: {ex.Message}");
}

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services.AddSingleton((s) =>
{
    string connectionString = Environment.GetEnvironmentVariable("CosmosDBConnection");
    return new CosmosClient(connectionString);
});

builder.Services.AddScoped<ITodoRepo, TodoRepo>();
builder.Services.AddScoped<ITodosService, TodoService>();

// Application Insights isn't enabled by default. See https://aka.ms/AAt8mw4.
// builder.Services
//     .AddApplicationInsightsTelemetryWorkerService()
//     .ConfigureFunctionsApplicationInsights();

builder.Build().Run();



// Function to create the database and container if they don't exist
async Task InitializeCosmosDBAsync(CosmosClient cosmosClient)
{
    // Create the database
    Database database = await cosmosClient.CreateDatabaseIfNotExistsAsync(databaseId);
    Console.WriteLine($"Database '{database.Id}' created or already exists.");

    // Create the container
    Container container = await database.CreateContainerIfNotExistsAsync(
        id: containerId,
        partitionKeyPath: partitionKeyPath,
        throughput: 400 // Provisioned throughput in RU/s
    );
    Console.WriteLine($"Container '{container.Id}' created or already exists.");
}

