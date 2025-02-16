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
builder.Services.AddScoped<IEventBus, EventBus>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        policy =>
        {
            policy.WithOrigins("http://127.0.0.1:5500", "http://localhost:5500")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
});



var app = builder.Build();
app.UseCors("AllowSpecificOrigin"); 
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// var eventBus = app.Services.GetRequiredService<IEventBus>();
// // eventBus.SubscribeDynamic<InitializeUserEventHandler>("InitializeUserEvent");
// //

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();