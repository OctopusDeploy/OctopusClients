using System;
using System.Collections.Generic;

namespace Octopus.Client.Model
{
    public interface IVariableTemplateContainer
    {
        List<ActionTemplateParameterResource> Templates { get; }
    }
}