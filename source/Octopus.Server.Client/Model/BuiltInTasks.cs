using System;

namespace Octopus.Client.Model
{
    public static class BuiltInTasks
    {
        public static string[] TasksThatCanBeQueuedByUsers()
        {
            // Everything except "Deploy", "Delete" and "ProcessExternalFeedTriggers"
            return new[]
            {
                Backup.Name, Health.Name, Retention.Name, Upgrade.Name, TestEmail.Name, AdHocScript.Name, UpdateCalamari.Name, TestAzureAccount.Name, SystemIntegrityCheck.Name, SyncCommunityActionTemplates.Name,
                Migration.Name, MigrationImport.Name, MigrationPartialExport.Name
            };
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

        public static class TaskRestrictedTo
        {
            public static readonly string DeploymentTargets = "DeploymentTargets";
            public static readonly string Workers = "Workers";
            public static readonly string Policies = "Policies";
            public static readonly string Unrestricted = "Unlimited";
        }

        public static class Health
        {
            public const string Name = "Health";

            public static string[] CanBeRestrictedTo()
            {
                return new string[]
                {
                    TaskRestrictedTo.DeploymentTargets,
                    TaskRestrictedTo.Workers,
                    TaskRestrictedTo.Policies,
                    TaskRestrictedTo.Unrestricted
                };
            }

            public static class Arguments
            {
                public static string EnvironmentId = "EnvironmentId";
                public static string WorkerpoolId = "WorkerpoolId";
                public static string MachineIds = "MachineIds";
                public static string RestrictedTo = "RestrictedTo";
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
                public const string ActionTemplateId = "ActionTemplateId";
                public const string Properties = "Properties";
                public const string TargetType = "TargetType";
            }
            
            public enum TargetType	
            {	
                Machines,	
                Environments,	
                Workers,	
                WorkerPools,	
                OctopusServer,	
            }
        }

        public static class Migration
        {
            public const string Name = "Migration";
        }

        public static class MigrationPartialExport
        {
            public const string Name = "MigrationPartialExport";
        }

        public static class MigrationImport
        {
            public const string Name = "MigrationImport";
        }

        public static class Retention
        {
            public const string Name = "Retention";
        }

        public static class Upgrade
        {
            public const string Name = "Upgrade";

            public static string[] CanBeRestrictedTo()
            {
                return new string[]
                {
                    TaskRestrictedTo.DeploymentTargets,
                    TaskRestrictedTo.Workers,
                    TaskRestrictedTo.Unrestricted
                };
            }

            public static class Arguments
            {
                public static string EnvironmentId = "EnvironmentId";
                public static string WorkerpoolId = "WorkerpoolId";
                public static string MachineIds = "MachineIds";
                public static string RestrictedTo = "RestrictedTo";
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

        public static class SyncCommunityActionTemplates
        {
            public const string Name = "SyncCommunityActionTemplates";
        }
        
        // Replaced by ProcessExternalFeedTriggers, will be removed shortly
        public static class PollFeedsForTriggers
        {
            public const string Name = "PollFeedsForTriggers";
        }
        
        public static class ProcessExternalFeedTriggers
        {
            public const string Name = "ProcessExternalFeedTriggers";
        }
    }
}