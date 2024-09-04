using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/movies")]
public class MovieController : ControllerBase
{
    private MovieService movieService;

    public MovieController(MovieService movieService)
    {
        this.movieService = movieService;
    }

    [HttpGet]
    public List<MovieDto> SearchMovies([FromQuery] string search) {
        return movieService.SearchMovies(search);
    }
}

public class MovieDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public DateTime ReleaseDate { get; set; }
    public string Genre { get; set; }
    public double Rating { get; set; }

    public List<string> Actors { get; set; }
}
