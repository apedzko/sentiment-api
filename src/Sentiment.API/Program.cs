using Sentiment.API.Infrastructure;
using Sentiment.API.Infrastructure.Azure;
using Sentiment.API.Infrastructure.Middleware;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddSwagger();

builder.Services.AddApplicationInsightsTelemetry();

builder.Services.AddStorageConfiguration(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger(app.Environment);

app.UseApiExceptionHandling();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
