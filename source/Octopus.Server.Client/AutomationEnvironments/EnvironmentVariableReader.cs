using System;

namespace Octopus.Client.AutomationEnvironments
{
    internal class EnvironmentVariableReader : IEnvironmentVariableReader
    {
        public string GetVariableValue(string name)
        {
            return Environment.GetEnvironmentVariable(name);
        }
    }
}