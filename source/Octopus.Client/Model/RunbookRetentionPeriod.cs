namespace Octopus.Client.Model
{
    public class RunbookRetentionPeriod
    {
        public int QuantityToKeep { get; set; }

        public bool ShouldKeepForever { get; set; }
    }
}