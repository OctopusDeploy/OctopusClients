#nullable enable
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Octopus.Client.Model.Itsm.Jira;

public class JiraServiceManagementIntegrationConnectionResource
{
    public string? Id { get; set; }

    [Description("Enter a name for this connection")]
    [DisplayName("Jira Service Management Connection Name")]
    [Required]
    public string? ConnectionName { get; set; }

    [Description("Enter the base url of your Jira Service Management instance.")]
    [DisplayName("Jira Service Management Base Url")]
    public string? BaseUrl { get; set; }

    [Description("Enter the username (email address) used to authenticate with Jira Service Management.")]
    [DisplayName("Jira Service Management Username")]
    public string? Username { get; set; }

    [Description("Enter the API token for your Jira Service Management account.")]
    [DisplayName("Jira Service Management user token")]
    public SensitiveValue? Token { get; set; }
}