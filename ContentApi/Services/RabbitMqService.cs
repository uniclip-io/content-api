using System.Text;
using ContentApi.Dtos;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace ContentApi.Services;

public class RabbitMqService : IDisposable
{
    private const string QueueName = "content-queue";
    private const string ExchangeName = "content-exchange";

    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitMqService(string hostName)
    {
        var factory = new ConnectionFactory { HostName = hostName };
        _connection = factory.CreateConnection();

        _channel = _connection.CreateModel();
        _channel.QueueDeclare(QueueName, true, false, false, null);
        _channel.ExchangeDeclare(ExchangeName, ExchangeType.Topic, true, false, null);
        _channel.QueueBind(QueueName, ExchangeName, "file.uploaded");
    }

    public void PublishFileUpload(FileContent fileContent)
    {
        var json = JsonConvert.SerializeObject(fileContent);
        var message = Encoding.UTF8.GetBytes(json);

        _channel.BasicPublish(ExchangeName, "file.uploaded", null, message);
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _channel.Close();
        _connection.Close();
    }
}