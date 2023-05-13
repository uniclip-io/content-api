using Bugsnag;
using ContentApi.Middlewares;
using ContentApi.Repository;
using ContentApi.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IClient>(_ => new Client(builder.Configuration["Bugsnag_ApiKey"]));
builder.Services.AddSingleton(_ => new ContentRepository(builder.Configuration["MongoDb_Connection"]!));
builder.Services.AddSingleton(_ => new EncryptionService(builder.Configuration["EncryptionKey"]!));
builder.Services.AddSingleton(_ => new RabbitMqService(
    builder.Configuration["RabbitMq_Username"]!, 
    builder.Configuration["RabbitMq_Password"]!, 
    builder.Configuration["RabbitMq_Uri"]!
));
builder.Services.AddTransient(s => new ContentService(
    s.GetService<ContentRepository>()!,
    s.GetService<RabbitMqService>()!,
    s.GetService<EncryptionService>()!
));

var app = builder.Build();
app.Services.GetService<ContentService>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<HttpExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();