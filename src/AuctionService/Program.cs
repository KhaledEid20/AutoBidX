global using Microsoft.EntityFrameworkCore;
global using AuctionService.Models;
using AuctionService.Data;
using AuctionService.Repository;
using MassTransit;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddDbContext<AuctionDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("AuctionDbConnection"))
);

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Register the UnitOfWork, BaseRepository, and AuctionRepository
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAuctionRepository, AuctionRepository>();
// register the auto mapper
builder.Services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));

// Adding the MassTransint(RabbitMQ)
builder.Services.AddMassTransit(x => 
{
    x.UsingRabbitMq((context , cfg) => {
        cfg.ConfigureEndpoints(context);
    });
} 
);

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

try{
    DbInitializer.Dbinit(app);
}
catch(Exception ex){
    Console.WriteLine(ex);
}

app.Run();
