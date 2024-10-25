using MongoDB.Driver;
using MongoDB.Entities;
using SearchService.Data;
using SearchService.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

try{
    DbInitializer.Dbinit(app);
}
catch(Exception e){
    Console.WriteLine($"cannot connect the database: \n {e}");
}

app.Run();