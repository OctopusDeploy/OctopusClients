using System;
using Newtonsoft.Json;

namespace Octopus.Client.Model;

public class RunbookRetentionPolicy
{
    public int QuantityToKeep { get; }

    public bool ShouldKeepForever => Strategy == RetentionPeriodStrategy.Forever || QuantityToKeep == 0;

    public RunbookRetentionUnit Unit { get; } 

    public RetentionPeriodStrategy Strategy { get;}

    [JsonConstructor]
    RunbookRetentionPolicy(RetentionPeriodStrategy strategy, int quantityToKeep, RunbookRetentionUnit unit)
    {
        Strategy = strategy ?? SelectStrategyBasedOnSettings(quantityToKeep, unit);
        QuantityToKeep = quantityToKeep;
        Unit = unit;
    }
    
    public static RunbookRetentionPolicy Forever() => new(RetentionPeriodStrategy.Forever, 0, RunbookRetentionUnit.Items);
    public static RunbookRetentionPolicy Default() => new(RetentionPeriodStrategy.Default,  0, RunbookRetentionUnit.Items);
    public static RunbookRetentionPolicy Count(int quantity, RunbookRetentionUnit unit) => new(RetentionPeriodStrategy.Count, quantity, unit);
    
    static RetentionPeriodStrategy SelectStrategyBasedOnSettings(int quantityToKeep, RunbookRetentionUnit unit) => quantityToKeep switch
    {
        0 => RetentionPeriodStrategy.Forever,
        100 when unit == RunbookRetentionUnit.Items => RetentionPeriodStrategy.Default,
        _ => RetentionPeriodStrategy.Count
    };
}
