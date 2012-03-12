using System;

namespace OctopusTools.Client
{
    public interface IOctopusSessionFactory
    {
        IOctopusSession OpenSession();
    }
}