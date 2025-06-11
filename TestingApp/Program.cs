using R.Tools.Requests;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.RegisterRequestsCollector();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.CollectRequests();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
