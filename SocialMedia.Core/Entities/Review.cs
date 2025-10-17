using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Core.Entities;

public partial class Review : BaseEntity
{
    //public int Id { get; set; }

    public int? UserId { get; set; }

    public int? MovieId { get; set; }
    public string? Description { get; set; }
    public DateTime Date { get; set; }
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual User? User { get; set; }

    public virtual Movie? Movie { get; set; }

    public float Grade { get; set; }

}

