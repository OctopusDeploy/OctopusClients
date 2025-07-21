using System;
using System.Collections.Generic;

namespace Octopus.Client.Model
{
    interface IVariableScopeValues
    {
        List<ReferenceDataItem> Environments { get; set; }
        List<ReferenceDataItem> Machines { get; set; }
        List<ReferenceDataItem> Actions { get; set; }
        List<ReferenceDataItem> Roles { get; set; }
        List<ReferenceDataItem> Channels { get; set; }
    }
}