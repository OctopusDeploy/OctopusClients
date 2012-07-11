using System;
using System.Text.RegularExpressions;

public static class PackageConstraintExtensions
{
    const string NugetPackageIdRegexPattern = "^[a-z0-9]+([_.-][a-z0-9]+)*$";

    public static string[] GetPackageIdAndVersion(string value)
    {
        if (!IsValidPackageConstraint(value))
        {
            throw new ArgumentException("'"+value+"' does not use expected format of : {PackageId}:{Version}");
        }

        var constraintSpecs = value.Split(':');
        
        return new string[]{
            constraintSpecs[0].Trim(),
            constraintSpecs[1].Trim()
            };
    }


    public static bool IsValidPackageConstraint(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Contains(" ") 
            || !value.Contains(":") || value.Split(':').Length != 2 || !IsValidVersionString(value.Split(':')[1]))
        {
            return false;
        }
        
        var constraintSpecs = value.Split(':');

        if (!IsValidNugetPackageId(constraintSpecs[0]))
        {
            return false;
        }
        if (!IsValidVersionString(constraintSpecs[1]))
        {
            return false;
        }
        return true;
    }

    public static bool IsValidNugetPackageId(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        return Regex.IsMatch(value.ToLower(), NugetPackageIdRegexPattern);
    }

    public static bool IsValidVersionString(string value)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            var versionComponents = value.Split('.');
            foreach (var s in versionComponents)
            {
                int.Parse(s);
            }
            return true;
        }
        catch (Exception ex)
        {
            return false;
        }
    }

}
