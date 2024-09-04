using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

public class MessageService : IHostedService
{
    private IConnection connection;
    private IModel channel;
    private IServiceProvider provider;
    private HttpClient httpClient;

    public MessageService(IServiceProvider provider)
    {
        this.provider = provider;
        this.httpClient = new HttpClient {
            BaseAddress = new Uri("http://localhost:5117")
        };
    }

    public void Connect()
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        connection = factory.CreateConnection();
        channel = connection.CreateModel();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Connect();
        ListenForMovieCreations();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        channel.Close();
        connection.Close();
        return Task.CompletedTask;
    }

    void ListenForMovieCreations()
    {
        channel.ExchangeDeclare(exchange: "create-movie", type: ExchangeType.Fanout);

        var queueName = channel.QueueDeclare("movie", true, false, false);
        channel.QueueBind(queue: queueName, exchange: "create-movie", routingKey: string.Empty);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var json = Encoding.UTF8.GetString(body);

            try
            {
                var movie = JsonSerializer.Deserialize<MovieDto>(json);
                Console.WriteLine("Created movie " + movie.Title);

                using (var scope = provider.CreateScope())
                {
                    var movieService = scope.ServiceProvider.GetRequiredService<MovieService>();
                    movieService.CreateMovie(movie);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        };

        channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
    }

    public MovieDto GetMovie(Guid id) {
        var webRequest = new HttpRequestMessage(HttpMethod.Get, "api/movies/" + id);

        var response = httpClient.Send(webRequest);

        using var reader = new StreamReader(response.Content.ReadAsStream());
        var json = reader.ReadToEnd();
        
        try
        {
            var movie = JsonSerializer.Deserialize<MovieDto>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            Console.WriteLine(movie.Title);

            return movie;
        }
        catch (Exception e)
        {
            System.Console.WriteLine("ERR:" + e.ToString());
        }

        return null;
    }
}
