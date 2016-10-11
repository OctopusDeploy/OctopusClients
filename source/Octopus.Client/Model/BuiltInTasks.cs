using System;

namespace Octopus.Client.Model
{
    public static class BuiltInTasks
    {
        public static string[] TasksThatCanBeQueuedByUsers()
        {
            // Everything except "Deploy" and "Delete"
            return new[] {Backup.Name, Health.Name, Retention.Name, Upgrade.Name, TestEmail.Name, AdHocScript.Name, UpdateCalamari.Name, TestAzureAccount.Name, SystemIntegrityCheck.Name, SyncLibrarySteps.Name};
        }

        public static class AutoDeploy
        {
            public const string Name = "AutoDeploy";

            public static class Arguments
            {
                public static string MachineIds = "MachineIds";
            }
        }

        public static class Backup
        {
            public static string Name = "Backup";
        }

        public static class Delete
        {
            public const string Name = "Delete";

            public static class Arguments
            {
                public static string DocumentId = "DocumentId";
            }
        }

        public static class Health
        {
            public const string Name = "Health";

            public static class Arguments
            {
                public static string EnvironmentId = "EnvironmentId";
                public static string MachineIds = "MachineIds";
                public static string Timeout = "Timeout";
                public static string MachineTimeout = "MachineTimeout";
            }
        }

        public static class AdHocScript
        {
            public const string Name = "AdHocScript";

            public static class Arguments
            {
                public const string EnvironmentIds = "EnvironmentIds"; // and
                public const string TargetRoles = "TargetRoles"; // or
                public const string MachineIds = "MachineIds";
                public const string ScriptBody = "ScriptBody";
                public const string Syntax = "Syntax";
            }
        }

        public static class Retention
        {
            public const string Name = "Retention";
        }

        public static class Upgrade
        {
            public const string Name = "Upgrade";

            public static class Arguments
            {
                public static string EnvironmentId = "EnvironmentId";
                public static string MachineIds = "MachineIds";
            }

        }
        public static class UpdateCalamari
        {
            public const string Name = "UpdateCalamari";

            public static class Arguments
            {
                public static string MachineIds = "MachineIds";
            }
        }

        public static class Deploy
        {
            public const string Name = "Deploy";

            public static class Arguments
            {
                public static string DeploymentId = "DeploymentId";
            }
        }

        public static class TestEmail
        {
            public const string Name = "TestEmail";

            public static class Arguments
            {
                public static string EmailAddress = "EmailAddress";
            }
        }

        public static class TestAzureAccount
        {
            public const string Name = "TestAzureAccount";

            public static class Arguments
            {
                public static string AccountId = "AccountId";
            }
        }

        public static class SystemIntegrityCheck
        {
            public const string Name = "SystemIntegrityCheck";
        }

        public static class SyncLibrarySteps
        {
            public const string Name = "SyncLibrarySteps";
        }
    }
}