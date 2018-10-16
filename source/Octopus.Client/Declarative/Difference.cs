namespace Octopus.Client.Declarative
{
    public class Difference
    {
        public Difference(string summary)
        {
            Summary = summary;
        }

        public string Summary { get; }

        public override string ToString()
        {
            return Summary;
        }
    }
}