public class Movie {
    public Guid Id { get; set; }
    public string Title {get; set; }
    public DateTime ReleaseDate { get; set;}
    public Genre Genre { get; set; }
    public double Rating { get; set; }

    public List<Actor> Actors { get; set; }
} 