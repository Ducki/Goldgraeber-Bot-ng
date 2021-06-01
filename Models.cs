namespace Bot_Dotnet.Models
{
    public class Triggers
    {
        public int id { get; set; }
        public string searchstring { get; set; }
    }

    class Answers
    {
        public int id { get; set; }
        public int trigger_id { get; set; }
        public string answer { get; set; }
    }
}