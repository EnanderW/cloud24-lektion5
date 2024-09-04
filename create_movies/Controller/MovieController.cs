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

    [HttpPost]
    // [Authorize("create_movie")]
    public ActionResult<MovieDto> CreateMovie([FromBody] CreateMovieDto dto)
    {
        try
        {
            Movie result = movieService.CreateMovie(dto);
            return Ok(new MovieDto(result));
        }
        catch (ArgumentException exception)
        {
            return BadRequest(exception.Message);
        }
    }

    [HttpGet("{id}")]
    public ActionResult<MovieDto> GetMovie(Guid id) {
        return Ok(new MovieDto(movieService.GetMovie(id)));
    }
}

public class CreateMovieDto
{
    public string Title { get; set; }
    public DateTime ReleaseDate { get; set; }
    public string Genre { get; set; }
    public double Rating { get; set; }

    public List<Guid> Actors { get; set; }
}

public class MovieDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public DateTime ReleaseDate { get; set; }
    public string Genre { get; set; }
    public double Rating { get; set; }

    public List<string> Actors { get; set; }

    public MovieDto(Movie movie)
    {
        this.Id = movie.Id;
        this.Title = movie.Title;
        this.Rating = movie.Rating;
        this.ReleaseDate = movie.ReleaseDate;
        this.Genre = movie.Genre.Id;
        this.Actors = movie.Actors.ConvertAll(value => value.Name).ToList();
    }
}
