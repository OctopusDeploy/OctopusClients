using System;

namespace Octopus.Client.Model
{
    public class RunbookRetentionPeriod
    {
        public RunbookRetentionPeriod(int quantityToKeep)
        {
            if (quantityToKeep < 0)
                throw new ArgumentOutOfRangeException(nameof(quantityToKeep), "Quantity to keep must be 0 (indicating keep forever) or greater");

            QuantityToKeep = quantityToKeep;
        }

        public int QuantityToKeep { get; }

        public bool ShouldKeepForever => QuantityToKeep == 0;

        public static RetentionPeriod KeepForever()
        {
            return new RetentionPeriod(0, RetentionUnit.Items);
        }
    }
}