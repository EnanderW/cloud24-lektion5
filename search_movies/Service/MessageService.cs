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

    // Anslut till RabbitMQ
    public void Connect()
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        connection = factory.CreateConnection();
        channel = connection.CreateModel();
    }

    // Anropas när programmet startas
    public Task StartAsync(CancellationToken cancellationToken)
    {
        Connect();
        ListenForMovieCreations();
        return Task.CompletedTask;
    }

    // Koppla bort när programmet stoppas
    public Task StopAsync(CancellationToken cancellationToken)
    {
        channel.Close();
        connection.Close();
        return Task.CompletedTask;
    }

    // Börja lyssna (consume) efter meddelanden från andra microservices
    void ListenForMovieCreations()
    {
        // Skapa/referera till samma exchange som i "create"-servicen
        channel.ExchangeDeclare(exchange: "create-movie", type: ExchangeType.Fanout);

        // Skapa/referera till en queue som håller alla meddelanden som skickas
        var queueName = channel.QueueDeclare("movie", true, false, false);
        channel.QueueBind(queue: queueName, exchange: "create-movie", routingKey: string.Empty);

        // Skapa en metod som anropas när ett meddelande kommer in (listener)
        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var json = Encoding.UTF8.GetString(body);

            try
            {
                var movie = JsonSerializer.Deserialize<MovieDto>(json);
                Console.WriteLine("Created movie " + movie.Title);

                // Skicka vidare informationen till "MovieService"
                // så att datan kan sparas i databas
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

        // Börja lyssna efter meddelanden (subscribe)
        channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
    }

    // Hämta en movie från en annan microservice
    // genom att skicka ett sync HTTP anrop
    public MovieDto GetMovie(Guid id) {
        // Skapa och skicka en request
        var webRequest = new HttpRequestMessage(HttpMethod.Get, "api/movies/" + id);

        var response = httpClient.Send(webRequest);

        // Läs in kropp i form av JSON och omvandla till objekt
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
