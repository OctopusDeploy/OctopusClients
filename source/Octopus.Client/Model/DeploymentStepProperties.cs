namespace Octopus.Client.Model
{
    public class DeploymentStepProperties
    {
        public enum HealthCheckType
        {
            FullHealthCheck,
            ConnectionTest
        }

        public enum HealthCheckErrorHandling
        {
            TreatExceptionsAsErrors,
            TreatExceptionsAsWarnings
        }

        public enum HealthCheckIncludeMachinesInDeployment
        {
            DoNotAlterMachines,
            IncludeCheckedMachines
        }
    }
}