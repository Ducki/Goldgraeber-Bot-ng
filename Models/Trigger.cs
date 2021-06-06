using System;
using System.Collections.Generic;

#nullable disable

namespace Bot_Dotnet
{
    public partial class Trigger
    {
        public Trigger()
        {
            Responses = new HashSet<Response>();
        }

        public long Id { get; set; }
        public string Searchstring { get; set; }

        public virtual ICollection<Response> Responses { get; set; }
    }
}
