using System;

namespace Octopus.Client.Model;

public class GitPersistenceSerializationFormat(string value) : IEquatable<GitPersistenceSerializationFormat>
{
    public static readonly GitPersistenceSerializationFormat Ocl = new("Ocl");
        
    string Value { get; } = value;
    
    public bool Equals(GitPersistenceSerializationFormat other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Value == other.Value;
    }

    public override bool Equals(object obj)
    {
        if (obj is null)
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj.GetType() != GetType())
        {
            return false;
        }

        return Equals((GitPersistenceSerializationFormat)obj);
    }

    public override int GetHashCode() => (Value != null ? Value.GetHashCode() : 0);
    
    public static bool operator ==(GitPersistenceSerializationFormat left, GitPersistenceSerializationFormat right)
        => object.Equals((object)left, (object)right);

    public static bool operator !=(GitPersistenceSerializationFormat left, GitPersistenceSerializationFormat right)
        => !object.Equals((object)left, (object)right);

    public override string ToString()
        => Value;
}
