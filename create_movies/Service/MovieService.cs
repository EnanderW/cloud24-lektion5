using Microsoft.EntityFrameworkCore;

public class MovieService {

    private ApplicationContext context;
    private MessageService messageService;

    public MovieService(ApplicationContext context, MessageService messageService) {
        this.context = context;
        this.messageService = messageService;
    }

    public Movie CreateMovie(CreateMovieDto dto) {
        if (dto.Title.Trim().Length == 0) {
            throw new ArgumentException("Title may not be empty.");
        }

        if (dto.Title.Length > 30) {
            throw new ArgumentException("Title may only be up to 30 characters.");
        }

        Genre genre = context.Genres
            .Where(value => value.Id.Equals(dto.Genre))
            .First();

        List<Actor> actors = dto.Actors
            .ConvertAll(actorId => {
                return context.Actors
                    .Include(value => value.Movies)
                    .Where(value => value.Id.Equals(actorId))
                    .First();
            })
            .ToList();

        Movie movie = new Movie {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            ReleaseDate = DateTime.UtcNow,
            Rating = dto.Rating,
            Actors = actors,
            Genre = genre,
        };

        foreach (Actor actor in actors) {
            actor.Movies.Add(movie);
        }

        context.Movies.Add(movie);
        context.SaveChanges();

        messageService.NotifyMovieCreation(new MovieDto(movie));

        return movie;
    }

    public Movie GetMovie(Guid id) {
        return context.Movies
            .Include(value => value.Actors)
            .Include(value => value.Genre)
            .Where(value => value.Id.Equals(id))
            .First();
    }
}