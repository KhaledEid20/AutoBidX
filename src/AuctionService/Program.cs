global using Microsoft.EntityFrameworkCore;
global using AuctionService.Models;
using AuctionService.Data;
using AuctionService.Repository;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;

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
    x.AddEntityFrameworkOutbox<AuctionDbContext>(o => {
        o.QueryDelay = TimeSpan.FromSeconds(10);
        o.UsePostgres();
        o.UseBusOutbox();
    });
    x.UsingRabbitMq((context , cfg) => {
        cfg.Host(builder.Configuration["RabbitMq:Host"] , "/" , host=>{ // This is the configuration of the rabbitmq we used it because of docker without docker it's run as default
            host.Username(builder.Configuration.GetValue("RabbitMq:Username" , "guest"));
            host.Password(builder.Configuration.GetValue("RabbitMq:Password" , "guest"));
        });
        cfg.ConfigureEndpoints(context);
    });
} 
);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o => {
        o.Authority = builder.Configuration["IdentityServiceUrl"];
        o.RequireHttpsMetadata = false;
        o.TokenValidationParameters.ValidateAudience = false;
        o.TokenValidationParameters.NameClaimType = "username";
    });

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

try{
    DbInitializer.Dbinit(app);
}
catch(Exception ex){
    Console.WriteLine(ex);
}

app.Run();