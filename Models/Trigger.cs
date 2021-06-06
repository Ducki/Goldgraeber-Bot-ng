using System;
using System.Collections.Generic;

#nullable disable

namespace Bot_Dotnet
{
    public partial class Trigger
    {
        public Trigger()
        {
            Answers = new HashSet<Answer>();
        }

        public long Id { get; set; }
        public string Searchstring { get; set; }

        public virtual ICollection<Answer> Answers { get; set; }
    }
}
