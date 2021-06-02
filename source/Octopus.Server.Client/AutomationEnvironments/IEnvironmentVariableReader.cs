namespace Octopus.Client.AutomationEnvironments
{
    internal interface IEnvironmentVariableReader
    {
        string GetVariableValue(string name);
    }
}