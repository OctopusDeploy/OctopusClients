using System.ComponentModel;
using Octopus.Client.Extensibility.Attributes;
using Octopus.Client.Extensibility.Extensions.Infrastructure.Configuration;

namespace Octopus.Client.Model.Authentication.Ldap
{
    public class LdapConfigurationResource : ExtensionConfigurationResource
    {
        public const string ServerDescription = "Set the server URL.";
        public const string PortDescription = "Set the port using to connect.";
        public const string SecurityProtocolDescription = "Sets the security protocol to use in securing the connection (None, StartTLS, or SSL).";
        public const string IgnoreSslErrorsDescription = "Sets whether to ignore certificate validation errors.";
        public const string UsernameDescription = "Set the user DN to query LDAP.";
        public const string PasswordDescription = "Set the password to query LDAP (leave empty for anonymous bind).";
        public const string UserBaseDnDescription = "Set the root distinguished name (DN) to query LDAP for Users.";
        public const string DefaultDomainDescription = "Set the default domain when none is given in the logon form. Optional.";
        public const string UserFilterDescription = "The filter to use when searching valid users.  '*' is replaced with a normalized version of the username.";
        public const string GroupBaseDnDescription = "Set the root distinguished name (DN) to query LDAP for Groups.";
        public const string GroupFilterDescription = "The filter to use when searching valid user groups.  '*' is replaced with the group name.";
        public const string AllowAutoUserCreationDescription = "Whether unknown users will be automatically created upon successful login.";
        public const string ReferralFollowingEnabledDescription = "Sets whether to allow referral following (this can slow down queries).";
        public const string ReferralHopLimitDescription = "Sets the maximum number of referrals to follow during automatic referral following.";
        public const string ConstraintTimeLimitDescription = "Sets the time limit in seconds for LDAP operations on the directory.  '0' specifies no limit.";
        public const string NestedGroupSearchDepthDescription = "Specifies how many levels of nesting will be searched. Set to '0' to disable searching for nested groups.";
        public const string NestedGroupFilterDescription = "The filter to use when searching for nested groups. '*' is replaced by the distinguished name of the initial group.";

        public LdapConfigurationResource()
        {
            Id = "authentication-ldap";
        }

        [DisplayName("Server")]
        [Description(ServerDescription)]
        [Writeable]
        public string Server { get; set; }

        [DisplayName("Port")]
        [Description(PortDescription)]
        [Writeable]
        public int Port { get; set; }

        [DisplayName("Security Protocol")]
        [Description(SecurityProtocolDescription)]
        [Writeable]
        public SecurityProtocol SecurityProtocol { get; set; }

        [DisplayName("Ignore SSL errors")]
        [Description(IgnoreSslErrorsDescription)]
        [Writeable]
        public bool IgnoreSslErrors { get; set; }

        [DisplayName("Username")]
        [Description(UsernameDescription)]
        [Writeable]
        public string ConnectUsername { get; set; }

        [DisplayName("Password")]
        [Description(PasswordDescription)]
        [Writeable]
        public SensitiveValue ConnectPassword { get; set; }

        [DisplayName("User Base DN")]
        [Description(UserBaseDnDescription)]
        [Writeable]
        public string UserBaseDN { get; set; }

        [DisplayName("Default Domain")]
        [Description(DefaultDomainDescription)]
        [Writeable]
        public string DefaultDomain { get; set; }

        [DisplayName("User Filter")]
        [Description(UserFilterDescription)]
        [Writeable]
        public string UserFilter { get; set; }

        [DisplayName("Group Base DN")]
        [Description(GroupBaseDnDescription)]
        [Writeable]
        public string GroupBaseDN { get; set; }

        [DisplayName("Group Filter")]
        [Description(GroupFilterDescription)]
        [Writeable]
        public string GroupFilter { get; set; }

        [DisplayName("Nested Group Filter")]
        [Description(NestedGroupFilterDescription)]
        [Writeable]
        public string NestedGroupFilter { get; set; }

        [DisplayName("Nested Group Search Depth")]
        [Description(NestedGroupSearchDepthDescription)]
        [Writeable]
        public int NestedGroupSearchDepth { get; set; }

        [DisplayName("Allow Auto User Creation")]
        [Description(AllowAutoUserCreationDescription)]
        [Writeable]
        public bool AllowAutoUserCreation { get; set; }

        [DisplayName("Enable Referral Following")]
        [Description(ReferralFollowingEnabledDescription)]
        [Writeable]
        public bool ReferralFollowingEnabled { get; set; }

        [DisplayName("Referral Hop Limit")]
        [Description(ReferralHopLimitDescription)]
        [Writeable]
        public int ReferralHopLimit { get; set; }

        [DisplayName("Constraint Time Limit")]
        [Description(ConstraintTimeLimitDescription)]
        [Writeable]
        public int ConstraintTimeLimit { get; set; }

        [DisplayName("Attribute Mapping")]
        public LdapMappingConfigurationResource AttributeMapping { get; set; } = new();
    }
}
