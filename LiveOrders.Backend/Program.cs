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

ConnectionOptions connection;

app.MapPost("/connection", (ConnectionOptions options) =>
    {
        if (string.IsNullOrEmpty(options.ApiKey))
            return Task.FromResult(Results.BadRequest("ApiKey must be not null or empty!"));
        if (options.ApiKey is "ApiKey" or "string")
            return Task.FromResult(Results.BadRequest("Incorrect ApiKey!"));
        connection = options;

        // run background task to push orders from tables to cache
        
        return Task.FromResult(Results.NoContent());
    })
    .WithName("PostConnectionOptions")
    .WithOpenApi();

app.MapGet("/orders", () =>
    {
        // return orders from cache
    });

app.Run();

record ConnectionOptions(string ApiKey, string OrgnizationId, string TerminalGroupId);