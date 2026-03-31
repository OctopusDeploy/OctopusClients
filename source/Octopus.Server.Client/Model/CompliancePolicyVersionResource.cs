using System;

namespace Octopus.Client.Model;

public class CompliancePolicyVersionResource
{
    public string Id { get; }
    public string Slug { get; }
    public string Version { get; }
    public DateTimeOffset PublishedDate { get; }
    public string GitRef { get; }
    public string GitCommit { get; }
    public string Name { get; }
    public string Description { get; }
    public string ViolationReason { get; }
    public string ViolationAction { get; }
    public string RegoScope { get; }
    public string RegoConditions { get; }
    public bool IsActive { get; }

    public CompliancePolicyVersionResource(
        string id,
        string slug,
        string version,
        DateTimeOffset publishedDate,
        string gitRef,
        string gitCommit,
        string name,
        string description,
        string violationReason,
        string violationAction,
        string regoScope,
        string regoConditions,
        bool isActive)
    {
        Id = id;
        Slug = slug;
        Version = version;
        PublishedDate = publishedDate;
        GitRef = gitRef;
        GitCommit = gitCommit;
        Name = name;
        Description = description;
        ViolationReason = violationReason;
        ViolationAction = violationAction;
        RegoScope = regoScope;
        RegoConditions = regoConditions;
        IsActive = isActive;
    }
}
