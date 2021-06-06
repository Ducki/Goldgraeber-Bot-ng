using System;
using System.Collections.Generic;

#nullable disable

namespace Bot_Dotnet
{
    public partial class Response
    {
        public long Id { get; set; }
        public long? TriggerId { get; set; }
        public string ResponseText { get; set; }

        public virtual Trigger Trigger { get; set; }
    }
}
