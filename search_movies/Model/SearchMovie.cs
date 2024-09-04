public class SearchMovie {
    public Guid Id { get; set; }
    public string Title {get; set; }
    public DateTime ReleaseDate { get; set;}
    public string Genre { get; set; }
    public double Rating { get; set; }

    public List<string> Actors { get; set; }
}