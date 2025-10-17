namespace Movies.Infrastructure.DTOs
{
    public class MovieDto
    {
        public int Id { get; set; }

        public string? Title { get; set; }

        public string Description { get; set; } = null!;

        public string? ReleaseDate { get; set; }

        public string Length { get; set; } = null!;

        public string Genre { get; set; } = null!;
    }
}
