namespace Movies.Infrastructure.DTOs
{
    public class ReviewDto
    {
        public int Id { get; set; }

        public int? UserId { get; set; }

        public int? MovieId { get; set; }
        public string? Description { get; set; }
        public string? Date { get; set; }
        public float Grade { get; set; }
    }
}
