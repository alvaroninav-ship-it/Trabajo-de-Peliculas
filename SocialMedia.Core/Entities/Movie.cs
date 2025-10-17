using System.Globalization;
using System.IO.Compression;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace Movies.Core.Entities;

public partial class Movie : BaseEntity
{
    //public int Id { get; set; }

    public string? Title { get; set; }
    
    public string? Description { get; set; }

    public DateTime ReleaseDate { get; set; }

    public virtual ICollection<Actor> Actors { get; set; } = new List<Actor>();

    public string Length { get; set; } = null!;

    public string Genre { get; set; } = null!;

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    

}

