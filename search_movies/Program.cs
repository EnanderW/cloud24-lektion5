using Microsoft.EntityFrameworkCore;

namespace lektion3;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddDbContext<ApplicationContext>(options =>
        {
            options.UseNpgsql(
                "Host=localhost;Database=micro-movie-search;Username=postgres;Password=password"
            );
        });

        builder.Services.AddScoped<MovieService>();
        builder.Services.AddHostedService<MessageService>();

        builder.Services.AddSingleton(x =>
            x.GetServices<IHostedService>().OfType<MessageService>().First()
        );
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.MapControllers();

        app.Run();
    }
}
