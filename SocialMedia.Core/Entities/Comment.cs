namespace Movies.Core.Entities;

public partial class Comment : BaseEntity
{
    //public int Id { get; set; }

    public int? ReviewId { get; set; }

    public int? UserId { get; set; }

    public string Description { get; set; } = null!;

    public DateTime Date { get; set; }

    public bool? IsActive { get; set; }

    public virtual Review? Review { get; set; }

    public virtual User? User { get; set; }
}
