﻿namespace Octopus.Client.Model
{
    public enum VariableType
    {
        String,
        Sensitive,
        Certificate,
        AmazonWebServicesAccount,
        AzureAccount,
        WorkerPool,
        UsernamePasswordAccount
    }
}