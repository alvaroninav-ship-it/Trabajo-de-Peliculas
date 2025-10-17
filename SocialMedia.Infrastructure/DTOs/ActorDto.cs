namespace Movies.Infrastructure.DTOs
{
    public class ActorDto
    {
        public int Id { get; set; }

        public int? MovieId { get; set; }

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? DateOfBirth { get; set; }

        public bool IsActive { get; set; }

    }
}
