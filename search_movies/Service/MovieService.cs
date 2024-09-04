public class MovieService {

    private ApplicationContext context;
    private MessageService messageService;

    public MovieService(ApplicationContext context, MessageService messageService) {
        this.context = context;
        this.messageService = messageService;
    }

    public List<MovieDto> SearchMovies(string search)
    {
        var result = context.Movies.Where(value => value.Title.Contains(search)).ToList();

        return result.ConvertAll(value => messageService.GetMovie(value.Id)).ToList();
    }

    // Lägger in en "kopia" av en film i databasen (varje microservice har en egen databas)
    public void CreateMovie(MovieDto dto) {
        var movie = new SearchMovie {
            Id = dto.Id,
            Title = dto.Title,
            ReleaseDate = dto.ReleaseDate,
            Rating = dto.Rating,
            Actors = dto.Actors,
            Genre = dto.Genre
        };

        context.Movies.Add(movie);
        context.SaveChanges();
    }
}