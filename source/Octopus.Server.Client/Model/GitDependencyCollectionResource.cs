#nullable enable
using System;
using System.Collections;
using System.Collections.Generic;

namespace Octopus.Client.Model;

public class GitDependencyCollectionResource : ICollection<GitDependencyResource>
{
    readonly Dictionary<string, GitDependencyResource> nameMap = new(StringComparer.OrdinalIgnoreCase);

    public GitDependencyCollectionResource()
    {
    }

    public GitDependencyCollectionResource(IEnumerable<GitDependencyResource> gitDependencies)
    {
        foreach (var gitDependency in gitDependencies)
        {
            Add(gitDependency);
        }
    }

    public GitDependencyResource? PrimaryGitDependency => nameMap.ContainsKey("") ? nameMap[""] : null;

    public bool HasPrimaryGitDependency => PrimaryGitDependency != null;

    public int Count => nameMap.Count;

    public bool IsReadOnly => false;

    public void Add(GitDependencyResource item)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        if (nameMap.ContainsKey(item.Name))
        {
            throw new ArgumentException($"A Git dependency with the name '{item.Name}' already exists");
        }

        nameMap.Add(item.Name, item);
    }

    public bool Contains(GitDependencyResource item) => nameMap.ContainsKey(item.Name);

    public void CopyTo(GitDependencyResource[] array, int arrayIndex)
    {
        nameMap.Values.CopyTo(array, arrayIndex);
    }

    public bool Remove(GitDependencyResource item) => nameMap.Remove(item.Name);

    public void Clear()
    {
        nameMap.Clear();
    }

    public IEnumerator<GitDependencyResource> GetEnumerator() => nameMap.Values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public GitDependencyResource GetByName(string name = "") => nameMap[name];
    
    public bool TryGetByName(string name, out GitDependencyResource? gitDependency)
    {
        var key = name ?? "";
        if (nameMap.ContainsKey(key))
        {
            gitDependency = nameMap[key];
            return true;
        }

        gitDependency = null;
        return false;
    }
}