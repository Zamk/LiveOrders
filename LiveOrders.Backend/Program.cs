using LiveOrders.Backend;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

ConnectionOptions connection = null;
ExternalClient client = null;
string uri = "https://api-ru.iiko.services/api/1/";


app.MapPost("/connection", (ConnectionOptions options) =>
    {
        connection = options;

        // run background task to push orders from tables to cache

        client = new ExternalClient(TimeSpan.FromSeconds(10), uri, connection.ApiKey);
        
        return Task.FromResult(Results.NoContent());
    })
    .WithName("PostConnectionOptions")
    .WithOpenApi();

app.MapGet("/tables", () =>
    {
        if (connection == null || client == null)
            throw new Exception();
        
        //var result = client.GetTables(connection.TerminalGroupId).Result;
        client.GetTablesTest();
        return Results.Ok();
    })
    .WithName("GetTables")
    .WithOpenApi();

app.MapGet("/orders", () =>
    {
        // return orders from cache
    })
    .WithName("GetOrders")
    .WithOpenApi();

app.Run();

record ConnectionOptions(string ApiKey, Guid OrgnizationId, Guid TerminalGroupId);