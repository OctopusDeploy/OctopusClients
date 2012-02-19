using System;
using System.Net;

namespace OctopusTools.Client
{
    public interface IOctopusSessionFactory
    {
        IOctopusSession OpenSession();
    }
}