using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movies.Core.CustomEntities
{
    public class PasswordOptions
    {
        public int SaltSiz { get; set; }
        public int KeySize { get; set; }

        public int Iterations { get; set; }
    }
}
