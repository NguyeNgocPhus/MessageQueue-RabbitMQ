using RabbitMQ.Configuration;
using RabbitMQ.RabbitMQ.Handler;
using RabbitMQ.RabbitMQ.Interfaces;
using RabbitMQ.RabbitMQ.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<RabbitMQConfiguration>(
    builder.Configuration.GetSection("RabbitMQ"));
builder.Services.AddSingleton<IEventBus, EventBus>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var eventBus = app.Services.GetRequiredService<IEventBus>();
eventBus.SubscribeDynamic<InitializeUserEventHandler>("InitializeUserEvent");


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();