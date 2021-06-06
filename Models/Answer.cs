using System;
using System.Collections.Generic;

#nullable disable

namespace Bot_Dotnet
{
    public partial class Answer
    {
        public long Id { get; set; }
        public long? TriggerId { get; set; }
        public string Answer1 { get; set; }

        public virtual Trigger Trigger { get; set; }
    }
}
