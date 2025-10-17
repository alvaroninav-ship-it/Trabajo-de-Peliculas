using System;
using System.Collections.Generic;

namespace Movies.Core.Entities;

public partial class User : BaseEntity
{
    //public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public DateTime DateOfBirth { get; set; }

    public string? Telephone { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
