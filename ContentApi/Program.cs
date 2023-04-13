using Bugsnag;
using ContentApi.Middlewares;
using ContentApi.Repository;
using ContentApi.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IClient>(_ => new Client(builder.Configuration["ApiKeys:Bugsnag"]));
builder.Services.AddSingleton(_ => new ContentRepository(builder.Configuration["ConnectionStrings:MongoDb"]!));
builder.Services.AddSingleton(_ => new RabbitMqService(builder.Configuration["ConnectionStrings:RabbitMq"]!));
builder.Services.AddTransient(s => new ContentService(
    s.GetService<ContentRepository>(),
    s.GetService<RabbitMqService>()
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