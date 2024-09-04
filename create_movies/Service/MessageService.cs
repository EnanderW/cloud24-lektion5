using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

public class MessageService : IHostedService {

    private IConnection connection;
    private IModel channel;

    public void Connect() {
        System.Console.WriteLine("HEJ!");
        var factory = new ConnectionFactory { HostName = "localhost" };
        connection = factory.CreateConnection();
        channel = connection.CreateModel();

        channel.ExchangeDeclare("create-movie", ExchangeType.Fanout);
    }

    public void NotifyMovieCreation(MovieDto movie) {
        var json = JsonSerializer.Serialize(movie);
        var message = Encoding.UTF8.GetBytes(json);

        channel.BasicPublish("create-movie", string.Empty, null, message);
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Connect();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        channel.Close();
        connection.Close();
        return Task.CompletedTask;
    }
}