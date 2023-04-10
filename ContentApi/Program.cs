using Bugsnag;
using ContentApi.Middlewares;
using ContentApi.Repository;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IClient>(_ => new Client(builder.Configuration["ApiKeys:Bugsnag"]));
builder.Services.AddSingleton(_ => new ContentRepository(builder.Configuration["ConnectionStrings:MongoDb"]!));

var app = builder.Build();

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